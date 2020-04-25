using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace theArch_LD46
{
    public class EnergyBar : MonoBehaviour
    {
        public Image BarFrameImage;

        public Image BlockImage0;
        public Image BlockImage1;
        public Image BlockImage2;
        public Image BlockImage3;
        public Image BlockImage4;
        public Image BlockImage5;
        public Image BlockImage6;
        public Image BlockImage7;
        public Image BlockImage8;
        public Image BlockImage9;

        private Image[] blockImages;

        void Awake()
        {
            blockImages = new[]
            {
                BlockImage0, BlockImage1,
                BlockImage2, BlockImage3,
                BlockImage4, BlockImage5,
                BlockImage6, BlockImage7,
                BlockImage8, BlockImage9
            };
        }

        public void SetBarFrameColor(Color col)
        {
            BarFrameImage.color = col;
        }

        public void SetBlockFrameColor(Color[] col)
        {
            if (blockImages != null)
            {
                for (int i = 0; i < blockImages.Length; i++)
                {
                    if (i < col.Length)
                    {
                        blockImages[i].color = col[i];
                    }
                    else
                    {
                        blockImages[i].color = Color.gray;
                    }
                }
            }
        }
    }
}