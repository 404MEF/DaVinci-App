namespace Davinci.Animations
{
    abstract class BaseAnimation
    {
        public abstract int AnimationIn
        {
            get;
        }
        public abstract int AnimationOut
        {
            get;
        }
        public abstract int PopAnimationIn
        {
            get;
        }
        public abstract int PopAnimationOut
        {
            get;
        }
    }
}