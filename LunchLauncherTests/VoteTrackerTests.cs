using LunchLauncher;
using LunchLauncher.Constraints;
using LunchLauncher.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchLauncherTests
{
    [TestClass]
    public class VoteTrackerTests
    {
        [TestMethod]
        public void it_keeps_track_of_votes()
        {
            var vt = new VoteTracker();
            vt.LogVote(new User(), new Restaurant());
            Assert.AreEqual(1, vt.TotalVotes);
        }

        [TestMethod]
        public void it_keeps_track_of_votes_per_user()
        {
            var vt = new VoteTracker();

            var r = new Restaurant("Jimmy Johns");
            var u = new User();

            vt.LogVote(u, r);
            Assert.AreEqual(1, vt.VotesForUser(u));
        }

        [TestMethod]
        public void it_keeps_track_of_votes_per_restaurant()
        {
            var vt = new VoteTracker();
            var r = new Restaurant("Gracie's");

            vt.LogVote(new User(), r);
            vt.LogVote(new User(), r);
            vt.LogVote(new User(), r);
            vt.LogVote(new User(), r);
            vt.LogVote(new User(), r);

            Assert.AreEqual(5, vt.VotesForRestaurant(r));
        }

        [TestMethod]
        public void it_keeps_track_of_votes_when_many_are_logged()
        {
            var vt = new VoteTracker();

            var u1 = new User("1");
            var u2 = new User("2");
            var u3 = new User("3");
            var u4 = new User("4");
            var u5 = new User("5");

            var r1 = new Restaurant("Gracie's");
            var r2 = new Restaurant("L&E");

            vt.LogVote(u1, r1);
            vt.LogVote(u2, r1);
            vt.LogVote(u3, r2);
            vt.LogVote(u4, r1);
            vt.LogVote(u5, r2);
            vt.LogVote(u1, r1);
            vt.LogVote(u2, r2);
            vt.LogVote(u3, r1);

            Assert.AreEqual(2, vt.VotesForUser(u1));
            Assert.AreEqual(2, vt.VotesForUser(u2));
            Assert.AreEqual(2, vt.VotesForUser(u3));
            Assert.AreEqual(1, vt.VotesForUser(u4));
            Assert.AreEqual(1, vt.VotesForUser(u5));

            Assert.AreEqual(5, vt.VotesForRestaurant(r1));
            Assert.AreEqual(3, vt.VotesForRestaurant(r2));

            Assert.AreEqual(8, vt.TotalVotes);
        }

        [TestMethod]
        [ExpectedException(typeof(ConstraintViolationException))]
        public void it_has_states_that_constrain_max_votes_per_user()
        {
            var u = new User();
            var r = new Restaurant();
            var MAX_VOTES_PER_USER = 5;

            var vt = new VoteTracker();
            var constraint = new MaxVotesPerUserConstraint(vt, MAX_VOTES_PER_USER);
            vt.SetConstraints(State.NominationPhase, constraint);

            for (var x = 0; x < 9; x++)
                vt.LogVote(u, r);

            Assert.Fail("exception not thrown");
        }

        [TestMethod]
        public void it_is_in_Nomination_state_when_initialized()
        {
            var vt = new VoteTracker();
            Assert.AreEqual(State.NominationPhase, vt.CurrentState);
        }

        [TestMethod]
        public void it_transitions_to_Selection_state_when_Nomination_ends()
        {
            var vt = new VoteTracker();
            vt.CloseNominationPhase();
            Assert.AreEqual(State.SelectionPhase, vt.CurrentState);
        }

        [TestMethod]
        public void it_allows_new_restaurants_during_Nomination_state()
        {
            var vt = new VoteTracker();
            vt.LogVote(new User(), new Restaurant());
            Assert.AreEqual(1, vt.TotalVotes);
        }

        [TestMethod]
        [ExpectedException(typeof(ConstraintViolationException))]
        public void it_does_not_allow_votes_for_new_restaurants_during_Selection_state()
        {
            var vt = new VoteTracker();
            var constraint = new NoNewRestaurantsConstraint(vt);
            vt.SetConstraints(State.SelectionPhase, constraint);

            vt.CloseNominationPhase();
            Assert.AreEqual(State.SelectionPhase, vt.CurrentState);

            vt.LogVote(new User(), new Restaurant());
            Assert.Fail("exception not thrown");
        }

        [TestMethod]
        public void it_calculates_top_three_restaurants_at_end_of_Nomination_state()
        {
            Assert.Fail("implement me");
        }
    }
}