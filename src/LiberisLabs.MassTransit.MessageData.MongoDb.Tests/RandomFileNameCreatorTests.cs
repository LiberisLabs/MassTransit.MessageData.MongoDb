using System.IO;
using System.Linq;
using NUnit.Framework;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.Tests
{
    [TestFixture]
    public class RandomFilenameCreatorTests
    {
        private string _filename1;
        private string _filename2;

        [SetUp]
        public void GivenARandomFileNameCreator_WhenCreatingTwoFilename()
        {
            var randomFileNameCreator = new RandomFileNameCreator();

            _filename1 = randomFileNameCreator.CreateFileName();
            _filename2 = randomFileNameCreator.CreateFileName();
        }

        [Test]
        public void ThenFilename1IsNotTheSameAsFilename2()
        {
            Assert.That(_filename1, Is.Not.EqualTo(_filename2));
        }


        [Test]
        public void ThenFilename1IsNotNullOrEmpty()
        {
            Assert.That(_filename1, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void ThenFilename2IsNotNullOrEmpty()
        {
            Assert.That(_filename1, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void ThenFilename1IsDoesNotContainInvalidCharacters()
        {
            foreach (char c in _filename1)
            {
                Assert.That(Path.GetInvalidFileNameChars().Contains(c), Is.False);
            }
        }

        [Test]
        public void ThenFilename2IsDoesNotContainInvalidCharacters()
        {
            foreach (char c in _filename2)
            {
                Assert.That(Path.GetInvalidFileNameChars().Contains(c), Is.False);
            }
        }
    }
}
