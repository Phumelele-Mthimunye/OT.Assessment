using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OT.Assessment.Tester
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerAdapter> _mockPlayerAdapter;
        private readonly PlayerService _playerService;
    }
}
