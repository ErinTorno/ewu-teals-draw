using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EwuTeals.Draw {
    abstract class ViewWatcher {
        private const int MaxQueueItems = 8;

        protected int displayCameraWidth, displayCameraHeight;
        protected Queue<Mat> disposableQueue = new Queue<Mat>();
        protected VideoCapture video;
        protected ImageBox canvas, videoBox, videoGreyBox;

        public ViewWatcher(VideoCapture video, ImageBox canvas, ImageBox videoBox, ImageBox videoGreyBox, int camW, int camH) {
            this.video = video;
            this.canvas = canvas;
            this.videoBox = videoBox;
            this.videoGreyBox = videoGreyBox;
            this.displayCameraHeight = camH;
            this.displayCameraWidth = camW;
        }

        abstract protected void ProcessFrameHook(Mat input);

        public void ProcessFrame() {
            if (video != null) {
                Mat videoFrame = video.QueryFrame(); // If not managed, video frame causes .2mb/s Loss, does not get cleaned up by GC. Must manually dispose.
                CvInvoke.Flip(videoFrame, videoFrame, FlipType.Horizontal);
                videoBox.Image = videoFrame;
                disposableQueue.Enqueue(videoFrame); // Add Video Frames to a queue to be disposed when NOT in use

                Mat combinedThreshImage = Mat.Zeros(videoFrame.Rows, videoFrame.Cols, DepthType.Cv8U, 1);
                disposableQueue.Enqueue(combinedThreshImage);

                // the subclasses override this to change what we do with the video capture
                ProcessFrameHook(videoFrame);

                videoGreyBox.Image = combinedThreshImage;

                if (disposableQueue.Count > 8) {
                    disposableQueue.Dequeue().Dispose();
                    disposableQueue.Dequeue().Dispose();
                }
            }
        }

        public Point ScaleToCanvas(Point point) {
            double widthMultiplier = (double)canvas.Width / displayCameraWidth;
            double heightMultiplier = (double)canvas.Height / displayCameraHeight;

            point.X = (int)(point.X * widthMultiplier);
            point.Y = (int)(point.Y * heightMultiplier);

            return point;
        }
    }
}
