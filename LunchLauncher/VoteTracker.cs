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
            _userVotes = new Dictionary<User, int>();
            _restaurantVotes = new Dictionary<Restaurant, int>();
            InitializeConstraints();
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
            ValidateConstraints(user, restaurant);
            LogVoteForUser(user);
            LogVoteForRestaurant(restaurant);
        }

        private void LogVoteForUser(User user)
        {
            if (_userVotes.ContainsKey(user))
                _userVotes[user] += 1;
            else
                _userVotes[user] = 1;
        }

        private void LogVoteForRestaurant(Restaurant restaurant)
        {
            if (_restaurantVotes.ContainsKey(restaurant))
                _restaurantVotes[restaurant] += 1;
            else
                _restaurantVotes[restaurant] = 1;
        }

        public int VotesForUser(User u)
        {
            if (_userVotes.ContainsKey(u))
                return _userVotes[u];

            return 0;
        }

        public int VotesForRestaurant(Restaurant restaurant)
        {
            return _restaurantVotes[restaurant];
        }

        public void SetConstraints(State state, BaseStateConstraint constraints)
        {
            _stateConstraints[state].Add(constraints);
        }

        private void ValidateConstraints(User user, Restaurant restaurant)
        {
            if (GetConstraintsForCurrentState()
                    .Select(constraint => constraint.Validate(user, restaurant))
                    .Any(isValid => !isValid))
                throw new ConstraintViolationException();
        }

        private IEnumerable<BaseStateConstraint> GetConstraintsForCurrentState()
        {
            IList<BaseStateConstraint> constraints;
            return _stateConstraints.TryGetValue(CurrentState, out constraints) ?
                                        constraints :
                                        null;
        }

        public void CloseNominationPhase()
        {
            TransitionToSelectionPhase();
        }

        private void TransitionToSelectionPhase()
        {
            CurrentState = State.SelectionPhase;
        }
    }
}