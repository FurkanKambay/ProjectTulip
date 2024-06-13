using UnityEngine.UIElements;

namespace Tulip.UI
{
    public static class DisplayExtensions
    {
        public static bool ToBool(this DisplayStyle display) => display == DisplayStyle.Flex;
        public static bool ToBool(this Visibility visibility) => visibility == Visibility.Visible;

        public static DisplayStyle ToDisplay(this bool value) => value ? DisplayStyle.Flex : DisplayStyle.None;
        public static Visibility ToVisibility(this bool value) => value ? Visibility.Visible : Visibility.Hidden;

        public static DisplayStyle ToDisplayInverse(this bool value) => (!value).ToDisplay();
        public static Visibility ToVisibilityInverse(this bool value) => (!value).ToVisibility();
    }
}
