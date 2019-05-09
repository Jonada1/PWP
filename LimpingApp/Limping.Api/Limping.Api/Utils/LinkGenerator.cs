using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL;
using Limping.Api.Constants;

namespace Limping.Api.Utils
{
    public static class LinkGenerator
    {
        public static class Users
        {
            private const string Relation = "users";
            public const string Prefix = "/api/users";
            public static Link GetAll(string rel = null)
            {
                return new Link(rel ?? $"{Relation}-all", Prefix, "Get all users", LinkMethods.GET);
            }
            public static Link Create(string rel = null)
            {
                return new Link(rel ?? $"{Relation}-create", Prefix, "Create user", LinkMethods.POST);
            }
            public static Link GetSingle(string id, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-single", $"{Prefix}/{id}", "Get user", LinkMethods.POST);
            }
            public static Link Delete(string id, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-delete", $"{Prefix}/{id}", "Delete user", LinkMethods.DELETE);
            }
            public static Link Edit(string id, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-edit", $"{Prefix}/{id}", "Edit user", LinkMethods.PATCH);
            }
        }
        public static class LimpingTests
        {
            private const string Relation = "limpingTests";
            public const string Prefix = "/api/limpingTests";
            public static Link GetAll(string rel = null)
            {
                return new Link(rel ?? $"{Relation}-all", Prefix, "Get all limping tests", LinkMethods.GET);
            }
            public static Link Create(string rel = null)
            {
                return new Link(rel ?? $"{Relation}-create", Prefix, "Create limping test", LinkMethods.POST);
            }
            public static Link GetSingle(string id, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-single", $"{Prefix}/{id}", "Get limping test", LinkMethods.GET);
            }
            public static Link GetForUser(string userId, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-single", $"{Prefix}/user/{userId}", "Get get limping tests for user", LinkMethods.GET);
            }
            public static Link Delete(string id, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-delete", $"{Prefix}/{id}", "Delete limping test", LinkMethods.DELETE);
            }
            public static Link Edit(string id, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-edit", $"{Prefix}/{id}", "Edit limping test", LinkMethods.PATCH);
            }
        }
        public static class Analysis
        {
            private const string Relation = "analysis";
            public const string Prefix = "/api/analysis";
            public static Link GetSingle(string id, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-single", $"{Prefix}/{id}", "Get analysis", LinkMethods.GET);
            }
            public static Link Edit(string id, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-edit", $"{Prefix}/{id}", "Edit analysis", LinkMethods.PUT);
            }
            public static Link Replace(string id, string rel = null)
            {
                return new Link(rel ?? $"{Relation}-replace", $"{Prefix}/replace/{id}", "Replace analysis", LinkMethods.PUT);
            }
        }
    }
}
