using Vayosoft.Persistence.Specifications;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Utilities;
using Xunit.Abstractions;

namespace Vayosoft.UnitTests
{
    public class SpecificationTests
    {
        private readonly ITestOutputHelper _output;
        public SpecificationTests(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public void IsSatisfiedByObject()
        {
            var spec = new UserIsActiveCriteria();
            var user = new User {IsActive = true};

            Assert.True(spec.IsSatisfiedBy(user));
        }

        [Fact]
        public void Combination()
        {
            var list = new List<User>
            {
                new User{ Username = "admin", IsActive = true },
                new User{ Username = "user", IsActive = true },
                new User{ Username = "support", IsActive = false},

            };

            var result = list.Where(new UserByUsernameCriteria("admin") && new UserIsActiveCriteria());
            _output.WriteLine(result.ToJson());
            //context.Users.Where(spec.ToExpression());
            //context.Uses.Where(new UserByUsernameCriteria("admin") || new UserByUsernameCriteria("user"));
        }

        public class UserIsActiveCriteria : Criteria<User>
        {
            public UserIsActiveCriteria()
            {
                Where(u => u.IsActive);
            }


        }

        public class UserByUsernameCriteria : Criteria<User>
        {
            public UserByUsernameCriteria(string username)
            {
                Where(u => u.Username == username);
            }
        }

        public class UserCombinedSpec : Specification<User>
        {
            public UserCombinedSpec(string login)
                : base(new UserIsActiveCriteria() && new UserByUsernameCriteria(login))
            {
            }
        }

        public class User
        {
            public string Username { get; set; } = null!;
            public bool IsActive { get; set; }
        }
    }
}
