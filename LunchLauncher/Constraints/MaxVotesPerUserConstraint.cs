namespace LunchLauncher.Constraints
{
    public class MaxVotesPerUserConstraint : BaseStateConstraint
    {
        private readonly int _maxVotesPerUser;

        public MaxVotesPerUserConstraint(VoteTracker voteTracker, int maxVotesPerUser)
            : base(voteTracker)
        {
            _maxVotesPerUser = maxVotesPerUser;
        }

        public override bool Validate(User user, Restaurant r)
        {
            return this.VoteTracker.VotesForUser(user) <= _maxVotesPerUser;
        }
    }
}