namespace LunchLauncher.Constraints
{
    public class NoNewRestaurantsConstraint : BaseStateConstraint
    {
        public NoNewRestaurantsConstraint(VoteTracker vt)
            : base(vt)
        { }

        public override bool Validate(User u, Restaurant r)
        {
            if (VoteTracker.Restaurants.Contains(r))
                return true;
            return false;
        }
    }
}