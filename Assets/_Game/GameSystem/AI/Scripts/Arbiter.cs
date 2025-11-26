using System;
using System.Collections.Generic;

namespace LOK1game.AI
{
    public class Arbiter
    {
        private readonly List<IExpert> _experts = new();

        public void RegisterExpert(IExpert expert) => _experts.Add(expert);
        public void UnregisterExpert(IExpert expert) => _experts.Remove(expert);
        public void ClearExperts() => _experts.Clear();

        public List<Action> BlackboardIteration(Blackboard blackboard)
        {
            IExpert bestExpert = null;
            int highestInsistence = 0;

            foreach (var expert in _experts)
            {
                var insistence = expert.GetInsistence(blackboard);

                if (insistence > highestInsistence)
                {
                    highestInsistence = insistence;
                    bestExpert = expert;
                }
            }

            bestExpert?.Execute(blackboard);

            var actions = new List<Action>(blackboard.PassedActions);
            blackboard.ClearActions();

            return actions;
        }
    }
}
