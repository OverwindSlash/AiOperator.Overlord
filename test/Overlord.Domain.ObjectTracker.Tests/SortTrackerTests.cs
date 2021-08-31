using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Overlord.Domain.ObjectTracker.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void SortTracker_FourEasyTracks_TrackedToEnd()
        {
            // Arrange
            var mot15Track = new List<Frame>{
                new Frame(new List<RectangleF>{
                    new RectangleF(1703,385,157,339),
                    new RectangleF(1293,455,83,213),
                    new RectangleF(259,449,101,261),
                    new RectangleF(1253,529,55,127)
                }),
                new Frame(new List<RectangleF>{
                    new RectangleF(1699,383,159,341),
                    new RectangleF(1293,455,83,213),
                    new RectangleF(261,447,101,263),
                    new RectangleF(1253,529,55,127)
                }),
                new Frame(new List<RectangleF>{
                    new RectangleF(1697,383,159,343),
                    new RectangleF(1293,455,83,213),
                    new RectangleF(263,447,101,263),
                    new RectangleF(1255,529,55,127),
                    new RectangleF(429,300,55,127)
                }),
                new Frame(new List<RectangleF>{
                    new RectangleF(1695,383,159,343),
                    new RectangleF(1293,455,83,213),
                    new RectangleF(265,447,101,263),
                    new RectangleF(1257,529,55,127)
                }),
                new Frame(new List<RectangleF>{
                    new RectangleF(1693,381,159,347),
                    new RectangleF(1295,455,83,213),
                    new RectangleF(267,447,101,263),
                    new RectangleF(1259, 529,55,129)
                }),
            };

            var tracks = Enumerable.Empty<Track>();
            var sut = new SortTracker();

            // Act
            foreach (var frame in mot15Track)
            {
                // ToArray because otherwise the IEnumerable is not evaluated.
                tracks = sut.Track(frame.BoundingBoxes).ToArray();
            }

            // Assert
            Assert.AreEqual(4, tracks.Where(x => x.State == TrackState.Active).Count());
        }
    }
}