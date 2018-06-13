using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using lmsextreg.Data;
using lmsextreg.Models;
using lmsextreg.Constants;

namespace lmsextreg.Pages.Approvals
{
    [Authorize(Roles = "APPROVER")]
    public class IndexModel: PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(lmsextreg.Data.ApplicationDbContext dbCntx, UserManager<ApplicationUser> usrMgr)
        {
            _dbContext = dbCntx;
            _userManager = usrMgr;
        }
        public IList<ProgramEnrollment> ProgramEnrollment { get;set; }
        public ApplicationUser LoggedInUser {get;set;}

        public async Task OnGetAsync()
        {
            Console.WriteLine("User is APPROVER: " + User.IsInRole(RoleConstants.APPROVER));

            LoggedInUser = await GetCurrentUserAsync();

            if ( User.IsInRole(RoleConstants.APPROVER))
            {
                var loggedInUserID = _userManager.GetUserId(User);
 
                var sql = " SELECT * FROM public.\"ProgramEnrollment\" "
                        + " WHERE \"LMSProgramID\" " 
                        + " IN "
                        + " ( "
                        + "   SELECT \"LMSProgramID\" "
                        + "   FROM public.\"ProgramApprover\" "
		                + "   WHERE \"ApproverUserId\" = {0} "
	                    + " ) ";

            ProgramEnrollment  = await _dbContext.ProgramEnrollments
                                .FromSql(sql, loggedInUserID)
                                .Include( pe =>  pe.LMSProgram)
                                .Include ( pe => pe.Student)
                                .Include( pe => pe.EnrollmentStatus)
                                .Include( pe => pe.Approver)
                                .OrderBy( pe => pe.LMSProgram.LongName)
                                    .ThenBy(pe => pe.Student.FullName)
                                    .ThenBy(pe => pe.EnrollmentStatus.StatusCode)
                                .ToListAsync();

                Console.WriteLine("ProgramEnrollment.Count: " + ProgramEnrollment.Count);
            }
            

        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}