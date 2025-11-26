using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOK1game.AI
{
    public interface IExpert
    {
        void ConstructExpert(Blackboard blackboard);
        int GetInsistence(Blackboard blackboard);
        void Execute(Blackboard blackboard);
    }
}
