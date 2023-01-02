using EmptyFiles;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Trivia;

namespace Tests
{
    internal class Game_Should
    {
        private TextWriter _currentConsoleOut;
        private StringWriter _stringWriter;

        private void Init()
        {
            _currentConsoleOut = Console.Out;
            _stringWriter = new StringWriter();
            Console.SetOut(_stringWriter);
        }

        [Test]
        public void NotBePlayable_Given_ThereAreNoPlayers()
        {
            var aGame = new Game();

            var playable = aGame.IsPlayable();

            Assert.That(playable, Is.False);
        }

        [Test]
        public void NotBePlayable_Given_ThereAreOnlyOnePlayer()
        {
            var aGame = new Game();

            aGame.Add("player");
            var playable = aGame.IsPlayable();

            Assert.That(playable, Is.False);
        }

        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void BePlayable_Given_ThereAreTwoOrMorePlayers(int numPlayers)
        {
            var aGame = new Game();

            for (int i = 0; i < numPlayers; i++)
            {
                aGame.Add("player" + i);
            }
            var playable = aGame.IsPlayable();

            playable.Should().BeTrue();
            Assert.That(playable, Is.True);
        }

        [Test]
        public void PrintNameOfAddedPlayer_When_AddingNewPlayer()
        {
            var aGame = new Game();

            aGame.Add("player");

            var output = GetOuput();
            Assert.That(output, Does.Contain("player was added"));
        }

        [Test]
        public void PrintPlayerIdOfAddedPlayer_When_AddingNewPlayer()
        {
            var aGame = new Game();

            aGame.Add("player");
            var output = GetOuput();
            var lines = output.Split(Environment.NewLine);

            lines[1].Should().Be("They are player number 1");
        }

        [Test]
        public void AlwaysReturnTrue_When_AddingNewPlayer()
        {
            var aGame = new Game();

            var result = aGame.Add("player");

            result.Should().BeTrue();
        }

        [TestCase(1)]
        [TestCase(42)]
        public void CreateRockQuestionWithCorrectIndex(int index)
        {
            var aGame = new Game();

            var result = aGame.CreateRockQuestion(index);

            result.Should().Be($"Rock Question {index}");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ReturnCorrectNumberOfP0ayers(int playersToAdd)
        {
            var aGame = new Game();

            for (int i = 0; i < playersToAdd; i++)
            {
                aGame.Add("");
            }

            var numberOfPlayers = aGame.HowManyPlayers();

            numberOfPlayers.Should().Be(playersToAdd);
        }

        [Test]
        public void PrintCorrectMessage_When_WrongAnswer()
        {
            Game aGame = StartTwoPlayerGame();

            aGame.WrongAnswer();
            var output = GetOuput();

            output.Should().Contain("Question was incorrectly answered");
        }

        [Test]
        public void AlwaysReturnTrue_When_CallingWrongAnswer()
        {
            var aGame = new Game();
            aGame.Add("player1");
            aGame.Add("player2");

            var result = aGame.WrongAnswer();

            result.Should().BeTrue();
        }

        [Test]
        public void PrintPlayer1WasSentToPenaltyBox_When_WrongAnswer()
        {
            var aGame = new Game();
            aGame.Add("player1");
            aGame.Add("player2");

            aGame.WrongAnswer();
            var output = GetOuput();

            output.Should().Contain("player1 was sent to the penalty box");
        }

        [Test]
        public void IncreaseCurrentPlayer_When_AnsweringIncorrectly()
        {
            Game aGame = StartTwoPlayerGame();

            aGame.WrongAnswer();
            aGame.WrongAnswer();

            var output = GetOuput();

            output.Should().Contain("player2 was sent to the penalty box");
        }

        [Test]
        public void PrintCurrentPlayer_When_CallingRoll()
        {
            Game aGame = StartTwoPlayerGame();

            aGame.Roll(1);
            var output = GetOuput();

            output.Should().Contain("player1 is the current player");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void PrintRollValue_When_CallingRoll(int roll)
        {
            Game aGame = StartTwoPlayerGame();

            aGame.Roll(roll);
            var output = GetOuput();

            output.Should().Contain("They have rolled a " + roll);
        }

        [Test]
        public void PrintNewPosition_When_CallingRoll()
        {
            Game aGame = StartTwoPlayerGame();

            aGame.Roll(1);
            var output = GetOuput();

            output.Should().Contain("player1's new location is 1");
        }

        [TestCase(1, "Science")]
        [TestCase(2, "Sports")]
        [TestCase(3, "Rock")]
        [TestCase(4, "Pop")]
        [TestCase(5, "Science")]
        [TestCase(6, "Sports")]
        [TestCase(7, "Rock")]
        [TestCase(8, "Pop")]
        [TestCase(9, "Science")]
        [TestCase(10, "Sports")]
        [TestCase(11, "Rock")]
        [TestCase(12, "Pop")]
        public void PrintCorrectQuestionCategory_When_CallingRoll(int roll, string category)
        {
            Game aGame = StartTwoPlayerGame();

            aGame.Roll(roll);
            var output = GetOuput();

            output.Should().Contain("The category is " + category);
        }

        [TestCase(1, "Science")]
        [TestCase(2, "Sports")]
        [TestCase(3, "Rock")]
        [TestCase(4, "Pop")]
        [TestCase(5, "Science")]
        [TestCase(6, "Sports")]
        [TestCase(7, "Rock")]
        [TestCase(8, "Pop")]
        [TestCase(9, "Science")]
        [TestCase(10, "Sports")]
        [TestCase(11, "Rock")]
        [TestCase(12, "Pop")]
        public void PrintCorrectQuestion_When_CallingRoll(int roll, string category)
        {
            Game aGame = StartTwoPlayerGame();

            aGame.Roll(roll);
            var output = GetOuput();

            output.Should().Contain(category + " Question 0");
        }

        [Test]
        public void NotPutPlayerInPenaltyBox_When_AddingNewPlayer()
        {
            Game aGame = StartTwoPlayerGame();

            aGame.Roll(42);
            var output = GetOuput();

            output.Should().NotContain("penalty box");
        }

        [Test]
        public void ReturnTrue_When_PlayerAnswersAnyQuestionCorrectly()
        {
            Game aGame = StartTwoPlayerGame();

            var result = aGame.WasCorrectlyAnswered();
            var output = GetOuput();

            result.Should().BeTrue();
        }

        [Test]
        public void ReturnFalse_When_PlayerAnswersSixQuestionsCorrectlyAndWinsTheGame()
        {
            Game aGame = StartTwoPlayerGame();

            bool result = false;
            for (int i = 0; i < 11; i++)
            {
                result = aGame.WasCorrectlyAnswered();
            }

            result.Should().BeFalse();
        }

        [Test]
        public void RunTwoPlayerGameUntilSomeoneWins()
        {
            Game aGame = StartTwoPlayerGame();
            bool isGameStillOn = true;
            int i = 0;
            while (isGameStillOn)
            {
                i++;
                aGame.Roll(i % 6);
                if (i % 5 == 0)
                    aGame.WrongAnswer();
                else
                    isGameStillOn = aGame.WasCorrectlyAnswered();
            }
            var output = GetOuput();

            output.Should().Be("player1 was added\r\nThey are player number 1\r\nplayer2 was added\r\nThey are player number 2\r\nplayer1 is the current player\r\nThey have rolled a 1\r\nplayer1's new location is 1\r\nThe category is Science\r\nScience Question 0\r\nAnswer was corrent!!!!\r\nplayer1 now has 1 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 2\r\nplayer2's new location is 2\r\nThe category is Sports\r\nSports Question 0\r\nAnswer was corrent!!!!\r\nplayer2 now has 1 Gold Coins.\r\nplayer1 is the current player\r\nThey have rolled a 3\r\nplayer1's new location is 4\r\nThe category is Pop\r\nPop Question 0\r\nAnswer was corrent!!!!\r\nplayer1 now has 2 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 4\r\nplayer2's new location is 6\r\nThe category is Sports\r\nSports Question 1\r\nAnswer was corrent!!!!\r\nplayer2 now has 2 Gold Coins.\r\nplayer1 is the current player\r\nThey have rolled a 5\r\nplayer1's new location is 9\r\nThe category is Science\r\nScience Question 1\r\nQuestion was incorrectly answered\r\nplayer1 was sent to the penalty box\r\nplayer2 is the current player\r\nThey have rolled a 0\r\nplayer2's new location is 6\r\nThe category is Sports\r\nSports Question 2\r\nAnswer was corrent!!!!\r\nplayer2 now has 3 Gold Coins.\r\nplayer1 is the current player\r\nThey have rolled a 1\r\nplayer1 is getting out of the penalty box\r\nplayer1's new location is 10\r\nThe category is Sports\r\nSports Question 3\r\nAnswer was correct!!!!\r\nplayer1 now has 3 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 2\r\nplayer2's new location is 8\r\nThe category is Pop\r\nPop Question 1\r\nAnswer was corrent!!!!\r\nplayer2 now has 4 Gold Coins.\r\nplayer1 is the current player\r\nThey have rolled a 3\r\nplayer1 is getting out of the penalty box\r\nplayer1's new location is 1\r\nThe category is Science\r\nScience Question 2\r\nAnswer was correct!!!!\r\nplayer1 now has 4 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 4\r\nplayer2's new location is 0\r\nThe category is Pop\r\nPop Question 2\r\nQuestion was incorrectly answered\r\nplayer2 was sent to the penalty box\r\nplayer1 is the current player\r\nThey have rolled a 5\r\nplayer1 is getting out of the penalty box\r\nplayer1's new location is 6\r\nThe category is Sports\r\nSports Question 4\r\nAnswer was correct!!!!\r\nplayer1 now has 5 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 0\r\nplayer2 is not getting out of the penalty box\r\nplayer1 is the current player\r\nThey have rolled a 1\r\nplayer1 is getting out of the penalty box\r\nplayer1's new location is 7\r\nThe category is Rock\r\nRock Question 0\r\nAnswer was correct!!!!\r\nplayer1 now has 6 Gold Coins.\r\n");
        }

        [Test]
        public void RunFivePlayerGameUntilSomeoneWins()
        {
            Init();
            var aGame = new Game();
            aGame.Add("player1");
            aGame.Add("player2");
            aGame.Add("player3");
            aGame.Add("player4");
            aGame.Add("player5");
            bool isGameStillOn = true;
            int i = 0;
            while (isGameStillOn)
            {
                i++;
                aGame.Roll(i % 6);
                if (i % 5 == 0)
                    aGame.WrongAnswer();
                else
                    isGameStillOn = aGame.WasCorrectlyAnswered();
            }
            var output = GetOuput();

            output.Should().Be("player1 was added\r\nThey are player number 1\r\nplayer2 was added\r\nThey are player number 2\r\nplayer3 was added\r\nThey are player number 3\r\nplayer4 was added\r\nThey are player number 4\r\nplayer5 was added\r\nThey are player number 5\r\nplayer1 is the current player\r\nThey have rolled a 1\r\nplayer1's new location is 1\r\nThe category is Science\r\nScience Question 0\r\nAnswer was corrent!!!!\r\nplayer1 now has 1 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 2\r\nplayer2's new location is 2\r\nThe category is Sports\r\nSports Question 0\r\nAnswer was corrent!!!!\r\nplayer2 now has 1 Gold Coins.\r\nplayer3 is the current player\r\nThey have rolled a 3\r\nplayer3's new location is 3\r\nThe category is Rock\r\nRock Question 0\r\nAnswer was corrent!!!!\r\nplayer3 now has 1 Gold Coins.\r\nplayer4 is the current player\r\nThey have rolled a 4\r\nplayer4's new location is 4\r\nThe category is Pop\r\nPop Question 0\r\nAnswer was corrent!!!!\r\nplayer4 now has 1 Gold Coins.\r\nplayer5 is the current player\r\nThey have rolled a 5\r\nplayer5's new location is 5\r\nThe category is Science\r\nScience Question 1\r\nQuestion was incorrectly answered\r\nplayer5 was sent to the penalty box\r\nplayer1 is the current player\r\nThey have rolled a 0\r\nplayer1's new location is 1\r\nThe category is Science\r\nScience Question 2\r\nAnswer was corrent!!!!\r\nplayer1 now has 2 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 1\r\nplayer2's new location is 3\r\nThe category is Rock\r\nRock Question 1\r\nAnswer was corrent!!!!\r\nplayer2 now has 2 Gold Coins.\r\nplayer3 is the current player\r\nThey have rolled a 2\r\nplayer3's new location is 5\r\nThe category is Science\r\nScience Question 3\r\nAnswer was corrent!!!!\r\nplayer3 now has 2 Gold Coins.\r\nplayer4 is the current player\r\nThey have rolled a 3\r\nplayer4's new location is 7\r\nThe category is Rock\r\nRock Question 2\r\nAnswer was corrent!!!!\r\nplayer4 now has 2 Gold Coins.\r\nplayer5 is the current player\r\nThey have rolled a 4\r\nplayer5 is not getting out of the penalty box\r\nQuestion was incorrectly answered\r\nplayer5 was sent to the penalty box\r\nplayer1 is the current player\r\nThey have rolled a 5\r\nplayer1's new location is 6\r\nThe category is Sports\r\nSports Question 1\r\nAnswer was corrent!!!!\r\nplayer1 now has 3 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 0\r\nplayer2's new location is 3\r\nThe category is Rock\r\nRock Question 3\r\nAnswer was corrent!!!!\r\nplayer2 now has 3 Gold Coins.\r\nplayer3 is the current player\r\nThey have rolled a 1\r\nplayer3's new location is 6\r\nThe category is Sports\r\nSports Question 2\r\nAnswer was corrent!!!!\r\nplayer3 now has 3 Gold Coins.\r\nplayer4 is the current player\r\nThey have rolled a 2\r\nplayer4's new location is 9\r\nThe category is Science\r\nScience Question 4\r\nAnswer was corrent!!!!\r\nplayer4 now has 3 Gold Coins.\r\nplayer5 is the current player\r\nThey have rolled a 3\r\nplayer5 is getting out of the penalty box\r\nplayer5's new location is 8\r\nThe category is Pop\r\nPop Question 1\r\nQuestion was incorrectly answered\r\nplayer5 was sent to the penalty box\r\nplayer1 is the current player\r\nThey have rolled a 4\r\nplayer1's new location is 10\r\nThe category is Sports\r\nSports Question 3\r\nAnswer was corrent!!!!\r\nplayer1 now has 4 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 5\r\nplayer2's new location is 8\r\nThe category is Pop\r\nPop Question 2\r\nAnswer was corrent!!!!\r\nplayer2 now has 4 Gold Coins.\r\nplayer3 is the current player\r\nThey have rolled a 0\r\nplayer3's new location is 6\r\nThe category is Sports\r\nSports Question 4\r\nAnswer was corrent!!!!\r\nplayer3 now has 4 Gold Coins.\r\nplayer4 is the current player\r\nThey have rolled a 1\r\nplayer4's new location is 10\r\nThe category is Sports\r\nSports Question 5\r\nAnswer was corrent!!!!\r\nplayer4 now has 4 Gold Coins.\r\nplayer5 is the current player\r\nThey have rolled a 2\r\nplayer5 is not getting out of the penalty box\r\nQuestion was incorrectly answered\r\nplayer5 was sent to the penalty box\r\nplayer1 is the current player\r\nThey have rolled a 3\r\nplayer1's new location is 1\r\nThe category is Science\r\nScience Question 5\r\nAnswer was corrent!!!!\r\nplayer1 now has 5 Gold Coins.\r\nplayer2 is the current player\r\nThey have rolled a 4\r\nplayer2's new location is 0\r\nThe category is Pop\r\nPop Question 3\r\nAnswer was corrent!!!!\r\nplayer2 now has 5 Gold Coins.\r\nplayer3 is the current player\r\nThey have rolled a 5\r\nplayer3's new location is 11\r\nThe category is Rock\r\nRock Question 4\r\nAnswer was corrent!!!!\r\nplayer3 now has 5 Gold Coins.\r\nplayer4 is the current player\r\nThey have rolled a 0\r\nplayer4's new location is 10\r\nThe category is Sports\r\nSports Question 6\r\nAnswer was corrent!!!!\r\nplayer4 now has 5 Gold Coins.\r\nplayer5 is the current player\r\nThey have rolled a 1\r\nplayer5 is getting out of the penalty box\r\nplayer5's new location is 9\r\nThe category is Science\r\nScience Question 6\r\nQuestion was incorrectly answered\r\nplayer5 was sent to the penalty box\r\nplayer1 is the current player\r\nThey have rolled a 2\r\nplayer1's new location is 3\r\nThe category is Rock\r\nRock Question 5\r\nAnswer was corrent!!!!\r\nplayer1 now has 6 Gold Coins.\r\n");
        }


        private Game StartTwoPlayerGame()
        {
            Init();
            var aGame = new Game();
            aGame.Add("player1");
            aGame.Add("player2");
            return aGame;
        }

        private string GetOuput()
        {
            return _stringWriter.ToString();
        }

        ~Game_Should()
        {
            Console.SetOut(_currentConsoleOut);
            _stringWriter.Dispose();
        }
    }
}
