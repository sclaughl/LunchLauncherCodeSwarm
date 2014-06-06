using System.Collections.Generic;
using System.Linq;
using LunchLauncher.Exceptions;

namespace LunchLauncher
{
    public class VoteTracker
    {
        private readonly IDictionary<User, int> _userVotes;
        private IDictionary<State, IList<BaseStateConstraint>> _stateConstraints;
        private readonly IDictionary<Restaurant, int> _restaurantVotes;

        public VoteTracker()
        {
            InitializeConstraints();
            _userVotes = new Dictionary<User, int>();
            _restaurantVotes = new Dictionary<Restaurant, int>();
        }

        private void InitializeConstraints()
        {
            _stateConstraints = new Dictionary<State, IList<BaseStateConstraint>>();
            _stateConstraints[State.NominationPhase] = new List<BaseStateConstraint>();
            _stateConstraints[State.SelectionPhase] = new List<BaseStateConstraint>();
        }

        public State CurrentState { get; private set; }

        public int TotalVotes { get { return _userVotes.Values.Sum(); } }

        public ICollection<Restaurant> Restaurants { get { return _restaurantVotes.Keys; } }

        public void LogVote(User user, Restaurant restaurant)
        {
            var constraints = GetConstraintsForCurrentState();
            foreach (var constraint in constraints)
            {
                var isValid = constraint.Validate(user, restaurant);
                if (!isValid) throw new ConstraintViolationException();
            }

            if (_userVotes.ContainsKey(user))
                _userVotes[user] += 1;
            else
                _userVotes[user] = 1;

            if (_restaurantVotes.ContainsKey(restaurant))
                _restaurantVotes[restaurant] += 1;
            else
                _restaurantVotes[restaurant] = 1;
        }

        private IEnumerable<BaseStateConstraint> GetConstraintsForCurrentState()
        {
            IList<BaseStateConstraint> constraints;
            return _stateConstraints.TryGetValue(CurrentState, out constraints) ?
                                        constraints :
                                        null;
        }

        public int VotesForUser(User u)
        {
            if (_userVotes.ContainsKey(u)) 
                return _userVotes[u];

            return 0;
        }

        public void CloseNominationPhase()
        {
            TransitionToSelectionPhase();
        }

        private void TransitionToSelectionPhase()
        {
            CurrentState = State.SelectionPhase;
        }

        public void SetConstraints(State state, BaseStateConstraint constraints)
        {
            _stateConstraints[state].Add(constraints);
        }

        public int VotesForRestaurant(Restaurant restaurant)
        {
            return _restaurantVotes[restaurant];
        }
    }
}