using System.Collections.Generic;
using System.Linq;
using LunchLauncher.Exceptions;

namespace LunchLauncher
{
    public class VoteTracker
    {
        private readonly IDictionary<User, int> _userVotes;
        private readonly IDictionary<Restaurant, int> _restaurantVotes;
        private IDictionary<State, IList<BaseStateConstraint>> _stateConstraints;

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
            return _userVotes.ContainsKey(u) ? _userVotes[u] : 0;
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
                                        new List<BaseStateConstraint>();
        }

        public IList<Restaurant> CloseNominationPhase()
        {
            var sortedRestaurants = SortRestaurantsByVotesDesc();
            TransitionToSelectionPhase();
            return sortedRestaurants.Take(3).ToList();
        }

        private IEnumerable<Restaurant> SortRestaurantsByVotesDesc()
        {
            return _restaurantVotes
                            .OrderByDescending(rv => rv.Value)
                            .Select(rv => rv.Key)
                            .ToList();
        }

        private void TransitionToSelectionPhase()
        {
            CurrentState = State.SelectionPhase;

            foreach (var u in _userVotes.Keys.ToList())
                _userVotes[u] = 0;

            foreach (var r in _restaurantVotes.Keys.ToList())
                _restaurantVotes[r] = 0;
        }
    }
}