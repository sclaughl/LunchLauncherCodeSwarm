namespace LunchLauncher
{
    public abstract class BaseStateConstraint
    {
        protected readonly VoteTracker VoteTracker;

        protected BaseStateConstraint(VoteTracker voteTracker)
        {
            VoteTracker = voteTracker;
        }

        public abstract bool Validate(User user, Restaurant r);
    }
}