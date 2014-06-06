using System;
using System.Collections.Generic;
using System.Linq;
using LunchLauncher.Exceptions;

namespace LunchLauncher
{
    public class VoteTracker
    {
        private readonly IDictionary<User, int> _userVotes;
        private readonly IDictionary<State, RoundConstraints> _stateConstraints;
        private readonly IDictionary<Restaurant, int> _restaurantVotes;

        public VoteTracker()
        {
            _stateConstraints = new Dictionary<State, RoundConstraints>();
            _userVotes = new Dictionary<User, int>();
            _restaurantVotes = new Dictionary<Restaurant, int>();
        }

        public State CurrentState { get; private set; }

        public int TotalVotes { get { return _userVotes.Values.Sum(); } }

        public void LogVote(User user, Restaurant restaurant)
        {
            var constraints = GetConstraintsForCurrentState();
            if (constraints != null)
                ValidateMaxVotesPerUserConstraint(user, constraints);

            if (_userVotes.ContainsKey(user))
                _userVotes[user] += 1;
            else
                _userVotes[user] = 1;

            if (_restaurantVotes.ContainsKey(restaurant))
                _restaurantVotes[restaurant] += 1;
            else
                _restaurantVotes[restaurant] = 1;
        }

        private RoundConstraints GetConstraintsForCurrentState()
        {
            RoundConstraints constraints;
            return _stateConstraints.TryGetValue(CurrentState, out constraints) ?
                                        constraints :
                                        null;
        }

        private void ValidateMaxVotesPerUserConstraint(User user, RoundConstraints constraints)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (constraints == null) throw new ArgumentNullException("constraints");

            if (_userVotes.ContainsKey(user) && _userVotes[user] >= constraints.MaxVotesPerUser)
                throw new UserExceededVotesException();
        }

        public int VotesForUser(User u)
        {
            return _userVotes[u];
        }

        public void CloseNominationPhase()
        {
            TransitionToSelectionPhase();
        }

        private void TransitionToSelectionPhase()
        {
            CurrentState = State.SelectionPhase;
        }

        public void SetConstraints(State state, RoundConstraints constraints)
        {
            _stateConstraints[state] = constraints;
        }

        public int VotesForRestaurant(Restaurant restaurant)
        {
            return _restaurantVotes[restaurant];
        }
    }
}