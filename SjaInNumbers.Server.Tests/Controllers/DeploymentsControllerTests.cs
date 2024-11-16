using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using SjaInNumbers.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaInNumbers.Server.Tests.Controllers;

public class DeploymentsControllerTests
{
    [Fact]
    public async Task GetNationalSummary_ReturnsSummaryFromService()
    {
        var controller = new DeploymentsController(
            Mock.Of<IDistrictService>(),
            Mock.Of<IDeploymentService>(),
            Mock.Of<IMapper>(),
            new FakeLogger<DeploymentsController>());
    }
}
