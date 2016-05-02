namespace Stash.rules
{
    // Support:
    //return new Cache().Which().IsThreadSafe()
				//  .With().MaximumSize(1000)
				//  .And().Expires().After(15 minutes).Or().At(12:00 AM)
                //      .WhicheverIsSooner()
				//  .And().CallsBackOnItemRemoval(callbackMethod);

    public static class CallbackOnRemoval
    {
    }
}