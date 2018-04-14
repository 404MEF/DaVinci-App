namespace Davinci.Animations
{
    class SlideRightLeft : BaseAnimation
    {
        public override int AnimationIn
        {
            get
            {
                return Resource.Animation.slide_from_right;
            }
        }
        public override int AnimationOut
        {
            get
            {
                return Resource.Animation.slide_to_left;
            }
        }
        public override int PopAnimationIn
        {
            get
            {
                return Resource.Animation.slide_from_left;
            }
        }
        public override int PopAnimationOut
        {
            get
            {
                return Resource.Animation.slide_to_right;
            }
        }
    }
}