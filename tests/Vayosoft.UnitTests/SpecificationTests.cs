using Vayosoft.Specifications;
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
            var spec = new UserIsActiveSpec();
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

            var result = list.Where(new UserByUsernameSpec("admin") && new UserIsActiveSpec());
            _output.WriteLine(result.ToJson());
            //context.Users.Where(spec.ToExpression());
            //context.Uses.Where(new UserByUsernameSpec("admin") || new UserByUsernameSpec("user"));
        }

        public class UserIsActiveSpec : Specification<User>
        {
            public UserIsActiveSpec()
            {
                Where(u => u.IsActive);
            }


        }

        public class UserByUsernameSpec : Specification<User>
        {
            public UserByUsernameSpec(string username)
            {
                Where(u => u.Username == username);
            }
        }

        public class UserCombinedSpec : Specification<User>
        {
            public UserCombinedSpec(string login)
                : base(new UserIsActiveSpec() && new UserByUsernameSpec(login))
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
