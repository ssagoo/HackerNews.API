using HackerNews.API.Application.Data;
using HackerNews.API.Application.Mappers;
using NUnit.Framework;

namespace HackerNews.API.Application.Tests.Mappers
{
    [TestFixture]
    public class HackerNewsStoryMapperTests
    {
        [Test]
        public void Map_WithAllValidFields()
        {
            var classUnderTest = new HackerNewsStoryMapper();

            var expectedTime = DateTimeOffset.UtcNow;
            var hackerNewsItem = new HackerNewsItem{by="user1", descendants = 10, score = 15, title = "title1", url = "http://www.someurl.com", time = expectedTime.ToUnixTimeSeconds(), id = 1};
            var result = classUnderTest.MapOut(new[] { hackerNewsItem }).ToList();

            Assert.That(result.Count, Is.EqualTo(1));

            var hackerNewsStoryDto = result[0];
            Assert.That(hackerNewsStoryDto.Title, Is.EqualTo(hackerNewsItem.title));
            Assert.That(hackerNewsStoryDto.Uri, Is.EqualTo(hackerNewsItem.url));
            Assert.That(hackerNewsStoryDto.PostedBy, Is.EqualTo(hackerNewsItem.by));
            Assert.That(hackerNewsStoryDto.Time, Is.EqualTo(ExpectedTimeLocalDateTime(expectedTime)));
            Assert.That(hackerNewsStoryDto.Score, Is.EqualTo(hackerNewsItem.score));
            Assert.That(hackerNewsStoryDto.CommentCount, Is.EqualTo(hackerNewsItem.descendants));
        }

        [Test]
        public void Map_WithNullStringFields()
        {
            var classUnderTest = new HackerNewsStoryMapper();

            var expectedTime = DateTimeOffset.UtcNow;
            var hackerNewsItem = new HackerNewsItem { by = null, descendants = 10, score = 15, title = null, url = "http://www.someurl.com", time = expectedTime.ToUnixTimeSeconds(), id = 1 };
            var result = classUnderTest.MapOut(new[] { hackerNewsItem }).ToList();

            Assert.That(result.Count, Is.EqualTo(1));

            var hackerNewsStoryDto = result[0];
            Assert.That(hackerNewsStoryDto.Title, Is.EqualTo(hackerNewsItem.title));
            Assert.That(hackerNewsStoryDto.Uri, Is.EqualTo(hackerNewsItem.url));
            Assert.That(hackerNewsStoryDto.PostedBy, Is.EqualTo(hackerNewsItem.by));
            Assert.That(hackerNewsStoryDto.Time, Is.EqualTo(ExpectedTimeLocalDateTime(expectedTime)));
            Assert.That(hackerNewsStoryDto.Score, Is.EqualTo(hackerNewsItem.score));
            Assert.That(hackerNewsStoryDto.CommentCount, Is.EqualTo(hackerNewsItem.descendants));
        }

        private static DateTime ExpectedTimeLocalDateTime(DateTimeOffset expectedTime)
        {
            var localDt = expectedTime.LocalDateTime;
            return new DateTime(localDt.Year, localDt.Month, localDt.Day, localDt.Hour, localDt.Minute, localDt.Second, DateTimeKind.Local);
        }
    }
}
