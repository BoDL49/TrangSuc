using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebTrangSuc.Views.JwtAuthorizeAttribute
{
    public class RoleAuthorizationAttribute : AuthorizeAttribute
    {
        private readonly int[] _allowedRoles;

        public RoleAuthorizationAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var userRole = actionContext.Request.Headers.Authorization?.Parameter;

            if (string.IsNullOrEmpty(userRole))
            {
                return false;
            }

            int role;
            if (!int.TryParse(userRole, out role))
            {
                return false;
            }

            // Kiểm tra role có nằm trong danh sách cho phép
            return _allowedRoles.Contains(role);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, "Bạn không có quyền truy cập.");
        }
    }
}