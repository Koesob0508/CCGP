using CCGP.AspectContainer;
using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class SerializedDataTests
    {

        [Test]
        public void SerializeCard()
        {
            // 1. 원본 Card 객체 생성
            Card originalCard = new Card
            {
                GUID = Guid.NewGuid().ToString(),
                OwnerIndex = 1,
                ID = "00-002",
                Name = "Signet Ring",
                Cost = 1,
                Persuasion = 1,
                Space = Space.Yellow | Space.Blue | Space.Green,
                Zone = Zone.Hand
            };

            var sCard = new SerializedCard(originalCard);

            // 2. FastBufferWriter를 사용하여 직렬화
            using (FastBufferWriter writer = new FastBufferWriter(1024, Unity.Collections.Allocator.Temp))
            {
                writer.WriteNetworkSerializable(sCard);

                // 3. FastBuffer를 사용하여 역직렬화
                using (FastBufferReader reader = new FastBufferReader(writer, Unity.Collections.Allocator.Temp))
                {
                    SerializedCard desCard = new();
                    reader.ReadNetworkSerializable(out desCard);

                    // 4. 검증
                    Assert.AreEqual(originalCard.GUID, desCard.GUID);
                    Assert.AreEqual(originalCard.OwnerIndex, desCard.OwnerIndex);
                    Assert.AreEqual(originalCard.ID, desCard.ID);
                    Assert.AreEqual(originalCard.Name, desCard.Name);
                    Assert.AreEqual(originalCard.Cost, desCard.Cost);
                    Assert.AreEqual(originalCard.Persuasion, desCard.Persuasion);
                    Assert.AreEqual(originalCard.Space, desCard.Space);
                    Assert.AreEqual(originalCard.Zone, desCard.Zone);
                }
            }
        }

        [Test]
        public void SerializedPlayer()
        {
            List<PlayerInfo> playerInfos = new()
            {
                new PlayerInfo(){ ClientID = 1, LobbyID = "Player1" },
                new PlayerInfo(){ ClientID = 2, LobbyID = "Player2" },
                new PlayerInfo(){ ClientID = 3, LobbyID = "Player3" },
                new PlayerInfo(){ ClientID = 4, LobbyID = "Player4" }
            };
            var game = new Game();
            game.PlayerInfos = playerInfos;
            var dataSystem = game.AddAspect<DataSystem>();
            game.Activate();

            var originalMatch = dataSystem.match;

            var originalPlayer = originalMatch.Players[0];
            var sPlayer = new SerializedPlayer(originalPlayer);

            // 2. FastBufferWriter를 사용하여 직렬화
            using (FastBufferWriter writer = new FastBufferWriter(1024, Unity.Collections.Allocator.Temp, maxSize: 8192))
            {
                writer.WriteNetworkSerializable(sPlayer);

                // 3. FastBuffer를 사용하여 역직렬화
                using (FastBufferReader reader = new FastBufferReader(writer, Unity.Collections.Allocator.Temp))
                {
                    SerializedPlayer desPlayer = new();
                    reader.ReadNetworkSerializable(out desPlayer);

                    // 4. 검증
                    Assert.AreEqual(originalPlayer.ID, desPlayer.ID);
                    Assert.AreEqual(originalPlayer.Index, desPlayer.Index);
                    Assert.AreEqual(originalPlayer[Zone.Deck].Count, desPlayer.Deck.Count);
                    Assert.AreEqual(originalPlayer[Zone.Hand].Count, desPlayer.Hand.Count);
                    Assert.AreEqual(originalPlayer[Zone.Graveyard].Count, desPlayer.Graveyard.Count);
                    Assert.AreEqual(originalPlayer[Zone.Deck][0].ID, desPlayer.Deck[0].ID);
                    Assert.AreEqual(originalPlayer[Zone.Deck][0].Space, desPlayer.Deck[0].Space);
                    Assert.AreEqual(originalPlayer[Zone.Deck][2].Space, desPlayer.Deck[2].Space);
                }
            }
        }

        public class TestTile : Tile { }

        [Test]
        public void SerializedMatch()
        {
            List<PlayerInfo> playerInfos = new()
            {
                new PlayerInfo(){ ClientID = 1, LobbyID = "Player1" },
                new PlayerInfo(){ ClientID = 2, LobbyID = "Player2" },
                new PlayerInfo(){ ClientID = 3, LobbyID = "Player3" },
                new PlayerInfo(){ ClientID = 4, LobbyID = "Player4" }
            };
            var game = new Game();
            game.PlayerInfos = playerInfos;
            var dataSystem = game.AddAspect<DataSystem>();
            game.Activate();

            var originalMatch = dataSystem.match;

            var tile1 = new TestTile() { Name = "Imperial Basin", Space = Space.Yellow };
            var tile2 = new TestTile() { Name = "Sietch Tabr", Space = Space.Blue };
            var tile3 = new TestTile() { Name = "Swordmaster", Space = Space.Green };

            originalMatch.Board.AddAspect(tile1, tile1.Name);
            originalMatch.Board.AddAspect(tile2, tile2.Name);
            originalMatch.Board.AddAspect(tile3, tile3.Name);

            var sMatch = new SerializedMatch(0, dataSystem.match);

            using (FastBufferWriter writer = new FastBufferWriter(4096, Unity.Collections.Allocator.Temp, maxSize: 8192))
            {
                writer.WriteNetworkSerializable(sMatch);

                // FastBuffer를 사용하여 역직렬화
                using (FastBufferReader reader = new FastBufferReader(writer, Unity.Collections.Allocator.Temp))
                {
                    SerializedMatch desMatch = new();
                    reader.ReadNetworkSerializable(out desMatch);

                    // 검증: Match의 기본 필드들
                    Assert.AreEqual(originalMatch.FirstPlayerIndex, desMatch.FirstPlayerIndex);
                    Assert.AreEqual(originalMatch.CurrentPlayerIndex, desMatch.CurrentPlayerIndex);
                    Assert.AreEqual(originalMatch.Players[0].ID, desMatch.Players[0].ID);
                    Assert.AreEqual(originalMatch.Players[originalMatch.Players.Count - 1][Zone.Deck].Count,
                                    desMatch.Players[desMatch.Players.Count - 1].Deck.Count);
                    Assert.AreEqual(originalMatch.Opened, desMatch.Opened);

                    // 검증: Board의 Tiles 데이터
                    Assert.AreEqual(originalMatch.Board.Tiles.Count, desMatch.Board.Tiles.Count);
                    for (int i = 0; i < originalMatch.Board.Tiles.Count; i++)
                    {
                        Assert.AreEqual(originalMatch.Board.Tiles[i].AgentIndex, desMatch.Board.Tiles[i].AgentIndex);
                        Assert.AreEqual(originalMatch.Board.Tiles[i].Space, desMatch.Board.Tiles[i].Space);
                    }
                }
            }
        }
    }
}