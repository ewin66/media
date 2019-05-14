using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace FutureConcepts.Media.Tools.NewLicense
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult Install(Session session)
        {
            session.Log("UnlockLeadTools.Install");
            try
            {
                LeadTools.Unlock();
            }
            catch (Exception e)
            {
                session.Log(e.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult Uninstall(Session session)
        {
            session.Log("UnlockLeadTools.Uninstall");
            try
            {
                LeadTools.Lock();
            }
            catch (Exception e)
            {
                session.Log("Error in NewLicense.Uninstall");
                session.Log(e.Message);
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult Rollback(Session session)
        {
            session.Log("UnlockLeadTools.Rollback");
            LeadTools.Lock();
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult Commit(Session session)
        {
            session.Log("UnlockLeadTools.Commit");
            return ActionResult.Success;
        }
    }
}
