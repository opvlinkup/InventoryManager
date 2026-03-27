namespace InventoryManager.Application.Helpers.Item;

public static class PatchHelper
{
        
        public static T? IfNotNull<T>(T? current, T? incoming, Func<T, T> transform) where T : class
        {
                return incoming != null ? transform(incoming) : current;
        }

        public static T? IfHasValue<T>(T? current, T? incoming) where T : struct
        {
                return incoming.HasValue ? incoming : current;
        }

}