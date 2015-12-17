using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shooter
{
    class Handler
    {
        public Tank[] tanks;
        public List<Obstacle> obstacles;
        public Client client;
        public String serverReply;
        public List<Coins> coinPiles;
        public List<LifePack> lifePacks;
        public String indicator;
        public Box[,] grid2;
        public String nextMove;
        public Vector2 position;
        public int playerNum;
        public List<Bullet> bullets;
        public SortedList<int,LinkedList<Box>> paths;

        public MyPathNode[,] grid;

        public Handler()
        {
            client = new Client();
            //tanks = new Tank[5];
            obstacles = new List<Obstacle>();
            coinPiles = new List<Coins>();
            lifePacks = new List<LifePack>();
            grid2 = new Box[21, 21];
            bullets = new List<Bullet>();
            paths = new SortedList<int,LinkedList<Box>>();
            grid = new MyPathNode[20, 20];
            serverReply = null;
        }

        public class MyPathNode : IPathNode<Object>
        {
            public Int32 X { get; set; }
            public Int32 Y { get; set; }
            public Boolean IsWall { get; set; }

            public bool IsWalkable(object usUsed)
            {
                return !IsWall;
            }
        }

        public class MySolver<TPathNode, TUserContext> : SpatialAStar<TPathNode, TUserContext> where TPathNode : IPathNode<TUserContext>
        {
            protected override Double Heuristic(PathNode inStart, PathNode inEnd)
            {
                return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
            }

            protected override Double NeighborDistance(PathNode inStart, PathNode inEnd, int aDirection)
            {
                return Heuristic(inStart, inEnd);
            }

            public MySolver(TPathNode[,] inGrid)
                : base(inGrid)
            {
            }
        }

        public Boolean join(Rectangle aScreen)
        {
            this.send("JOIN#");
            //serverReply = this.receive();
            return this.receive(aScreen);

            //if (serverReply.ElementAt<char>(0)=='I')
            //{
            //    createObstacles();
            //    createTanks(aScreen);
            //    return true;
            //}
            //else
            //{   
            //    return false;
            //}
        }
        public void createTanks(Rectangle gameScreen)
        {
            /*for (int i = 0; i < 5; i++)
            {
                tanks[i] = new Tank(i);

            }
            tanks[0].color = Color.Blue;
            tanks[1].color = Color.Green;
            tanks[2].color = Color.Red;
            tanks[3].color = Color.Yellow;
            tanks[4].color = Color.Magenta;

            tanks[0].position = new Vector2(0 + gameScreen.Left, 0 + gameScreen.Top);
            tanks[1].position = new Vector2(0 + gameScreen.Left, 570 + gameScreen.Top);
            tanks[4].position = new Vector2(300 + gameScreen.Left, 300 + gameScreen.Top);
            tanks[2].position = new Vector2(570 + gameScreen.Left, 0 + gameScreen.Top);
            tanks[3].position = new Vector2(570 + gameScreen.Left, 570 + gameScreen.Top);
            */
            Color[] colors = new Color[] { Color.Blue, Color.Green, Color.Red, Color.Yellow, Color.Magenta };

            String[] players = serverReply.Split(':');
            tanks = new Tank[players.Length - 1];
            for (int i = 1; i < players.Length; i++)
            {
                String[] playerParts = players[i].Split(';');
                String[] playerPos = playerParts[1].Split(',');
                tanks[i - 1] = new Tank(players[i].ElementAt(1) - 48, colors[i - 1], new Vector2(int.Parse(playerPos[0]), int.Parse(playerPos[1])), players[i].ElementAt(players[i].Length - 1) - 48);
                //Console.WriteLine(tanks[i-1].direction);
            }
        }

        public Boolean receive(Rectangle aScreen)
        {
            String reply = this.client.receive();
            Console.WriteLine(reply);
            indicator = reply.Substring(0, 2);
            if (indicator == "I:" || indicator == "G:" || indicator == "S:" || indicator == "C:" || indicator == "L:")
            {
                serverReply = reply;
                return this.processReply(aScreen);
            }
            else
            {
                return false;
            }
            //return reply;
        }

        public Boolean processReply(Rectangle aScreen)
        {
            indicator = serverReply.Substring(0, 2);
            if (indicator == "I:" || indicator == "G:" || indicator == "S:" || indicator == "C:" || indicator == "L:")
            {
                if (indicator == "I:")
                {
                    //createTanks(aScreen);
                    playerNum = serverReply.ElementAt<char>(serverReply.IndexOf('P') + 1) - 48;
                    createObstacles();
                }
                else if (indicator == "G:")
                {
                    //updateTanks();
                    //updateObstacles();
                    update();
                }
                else if (indicator == "S:")
                {
                    createTanks(aScreen);
                }
                else if (indicator == "C:")
                {
                    createCoinPile();
                }
                else
                {
                    createLifePack();
                }
                //createObstacles();
                //createTanks(aScreen);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void createCoinPile()
        {
            String[] parts = serverReply.Split(':');
            String[] pos = parts[1].Split(',');
            coinPiles.Add(new Coins(int.Parse(parts[2]), int.Parse(parts[3]), new Vector2(int.Parse(pos[0]), int.Parse(pos[1]))));
            
        }

        

        public int updateCoinPile(int aTime)
        {
            int count = coinPiles.Count;
            for (int i = 0; i < count; i++)
            {
                Coins pile = coinPiles.ElementAt(i);
                if (pile != null)
                {
                    pile.spentTime += aTime;
                    if (pile.spentTime >= pile.lifeTime)
                    {
                        pile.isAlive = false;
                        coinPiles.Remove(pile);
                        count--;
                    }
                    for (int j = 0; j < tanks.Length; j++)
                    {
                        if (tanks[j].position == pile.position)
                        {
                            pile.isAlive = false;
                            coinPiles.Remove(pile);
                            count--;
                        }
                    }
                }
            }
            return coinPiles.Count;
        }

        public void killTank(int i){
                this.tanks[i].status = 1;
             
        }

        public int updateLifePacks(int aTime)
        {
            int count = lifePacks.Count;
            for (int i = 0; i < count; i++)
            {
                LifePack pack = lifePacks.ElementAt(i);
                if (pack != null)
                {
                    pack.spentTime += aTime;
                    if (pack.spentTime >= pack.lifeTime)
                    {
                        pack.isAlive = false;
                        lifePacks.Remove(pack);
                        count--;
                    }
                    for (int j = 0; j < tanks.Length; j++)
                    {
                        if (tanks[j].position == pack.position)
                        {
                            pack.isAlive = false;
                            lifePacks.Remove(pack);
                            count--;
                        }
                    }
                }
            }
            return lifePacks.Count;
        }


        public void createLifePack()
        {
            String[] parts = serverReply.Split(':');
            String[] pos = parts[1].Split(',');
            //int count = lifePacks.Count;
            /*for (int i = 0; i < lifePacks.Count; i++)
            {
                LifePack pack = lifePacks.ElementAt(i);
                if (!pack.isAlive)
                {
                    lifePacks.Remove(pack);
                }
            }*/
            lifePacks.Add(new LifePack(int.Parse(parts[2]), new Vector2(int.Parse(pos[0]), int.Parse(pos[1]))));
        }

        public void moveTank()
        {
            this.findPath();
            this.findNext();
            client.send(nextMove);
            client.receive();
        }


        public void shoot()
        {
            client.send("SHOOT#");
            client.receive();
        }

        public void update()
        {
            String[] parts = serverReply.Split(':');
            String[] playerParts = new String[7];
            for (int i = 1; i < parts.Length; i++)
                if (parts[i].ElementAt(0) == 'P')
                {
                    playerParts = parts[i].Split(';');
                    grid[(int)tanks[i - 1].position.X, (int)tanks[i - 1].position.Y].IsWall = false;
                    String[] positionArray = playerParts[1].Split(',');
                    Vector2 pos = new Vector2(int.Parse(positionArray[0]), int.Parse(positionArray[1])); // the position of a bullet is taken in pixels
                    tanks[i - 1].position = pos;
                    //grid2[(int)tanks[i - 1].position.X, (int)tanks[i - 1].position.Y] = new Box(1000,1000, pos);

                    grid[(int)tanks[i - 1].position.X, (int)tanks[i - 1].position.Y] = new MyPathNode()
                    {
                        IsWall = true,
                        X = (int)tanks[i - 1].position.X,
                        Y=(int)tanks[i - 1].position.Y,
                    };
                    
                    
                    int dir = Int32.Parse(playerParts[2]);
                    tanks[i - 1].direction = dir;
                    if (playerParts[3] == "0")
                    {
                        tanks[i - 1].hasShot = false;
                    }
                    else
                    {
                        tanks[i - 1].hasShot = true;
                        updateBullets();
                        Vector2 bulletPosition;
                        switch (dir)
                        {
                            case 0:
                                {
                                    bulletPosition = new Vector2(pos.X * 30, pos.Y * 30 - 30);
                                    break;
                                }
                            case 1:
                                {
                                    bulletPosition = new Vector2(pos.X * 30 + 30, pos.Y * 30);
                                    break;
                                }
                            case 2:
                                {
                                    bulletPosition = new Vector2(pos.X * 30, pos.Y * 30 + 30);
                                    break;
                                }
                            case 3:
                                {
                                    bulletPosition = new Vector2(pos.X * 30 - 30, pos.Y * 30);
                                    break;
                                }
                            default:
                                {
                                    bulletPosition = new Vector2(0, 0);
                                    break;
                                }
                        }

                        bullets.Add(new Bullet(dir, bulletPosition));
                        
                    }
                    tanks[i - 1].health = Int32.Parse(playerParts[4]);
                    tanks[i - 1].coins = Int32.Parse(playerParts[5]);
                    tanks[i - 1].points = Int32.Parse(playerParts[6]);
                    if (tanks[i - 1].health == 0)
                    {
                        if (tanks[i - 1].status == 0)
                        {
                            this.killTank(i - 1);
                        }
                        else if (tanks[i - 1].status == 1)
                        {
                            tanks[i - 1].status = 2;
                        }
                    }

                }
                else
                {
                    String[] obPos = parts[i].Split(';');
                    String[] posDir;
                    for (int j = 0; j < obPos.Length; j++)
                    {
                        posDir = obPos[j].Split(',');
                        int damageLevel = Int32.Parse(posDir[2]);
                        if (damageLevel == 100)
                        {
                            obstacles.RemoveAt(j);
                        }
                        else { 
                            obstacles.ElementAt(j).damageLevel = damageLevel; 
                        }

                    }
                }
            position = tanks[playerNum].position;
            Boolean inLineX = false;
            Boolean inLineY = false;
            List<Tank> inLineXTanks= new List<Tank>();
            List<Tank> inLineYTanks= new List<Tank>();

            for (int i = 0; i < tanks.Length; i++)
            {
                if (i == playerNum || tanks[i].health == 0)
                {
                    continue;
                }
                    if (position.X == tanks[i].position.X && Math.Abs(position.Y-tanks[i].position.Y) <5)
                    {
                        inLineX = true;
                        inLineXTanks.Add(tanks[i]);
                    }
                    else if (position.Y == tanks[i].position.Y  && Math.Abs(position.X-tanks[i].position.X) <5)
                    {
                        inLineY = true;
                        inLineYTanks.Add(tanks[i]);
                    }
                
            }
            if (inLineX == true)
            {
                foreach(Tank tank in inLineXTanks)
                {
                    if (tank.position.Y < position.Y)
                    {
                        if(tanks[playerNum].direction==0){
                            shoot();
                        }
                        else{
                            client.send("UP#");
                            client.receive();
                        }
                    }
                    if (tank.position.Y > position.Y)
                    {
                        if (tanks[playerNum].direction == 2)
                        {
                            shoot();
                        }
                        else
                        {
                            client.send("DOWN#");
                            client.receive();
                        }
                    }

                }      
            }
            else if (inLineY == true)
            {
                foreach (Tank tank in inLineYTanks)
                {
                    if (tank.position.X < position.X)
                    {
                        if (tanks[playerNum].direction == 3)
                        {
                            shoot();
                        }
                        else
                        {
                            client.send("LEFT##");
                            client.receive();
                        }
                    }
                    if (tank.position.X > position.X)
                    {
                        if (tanks[playerNum].direction == 1)
                        {
                            shoot();
                        }
                        else
                        {
                            client.send("RIGHT#");
                            client.receive();
                        }
                    }

                }
            }
            else
            {
                moveTank();
            }
        }




        public void computeCost(int source_x, int source_y, int dest_x, int dest_y)
        {
            Box source = grid2[source_x, source_y];
            for (int k = source_x - 1; k <= source_x + 1; k = k + 2)
            {
                for (int l = source_y - 1; l <= source_y + 1; l = l + 2)
                {
                    if (k == source_x && l == source_y)
                    {
                        source.f = 1;
                        source.computed = true;
                    }
                    else
                    {
                        Box b = grid2[k, l];
                        if (b.computed == true)
                        {
                            source.f = b.f + 1;
                            source.computed = true;
                        }
                        else if (b.computing == true)
                        {

                        }
                        else
                        {
                            computeCost(k, l, dest_x, dest_y);
                        }
                    }
                }
            }
        }

        public void findNext(int dest_x, int dest_y)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            Box[] steps = new Box[4];
            nextMove = "LEFT#";
            Boolean found = false;
            int cost = mod(dest_x - x + dest_y - y);
            for (int i = 0; i < this.tanks.Length && i!=playerNum; i++)
            {
                if (x == tanks[i].position.X || y == tanks[i].position.Y)
                {
                    nextMove="SHOOT#";
                    found = true;
                }
            }
            if (!found)
            {
                for (int k = x - 1; k <= x + 1; k = k + 2)
                {
                    if (k == -1 || k == 21 || (grid2[k, y] != null && grid2[k, y].f < 0))
                    {
                        continue;
                    }

                    grid2[k, y] = new Box();
                    int newCost = mod(dest_x - k + dest_y - y);
                    grid2[k, y].f = newCost;
                    if (newCost < cost)
                    {
                        cost = newCost;

                    }
                }

                for (int l = y - 1; l <= y + 1; l = l + 2)
                {
                    if (l == -1 || l == 21 || (grid2[x, l] != null && grid2[x, l].f < 0))
                    {
                        continue;
                    }
                    grid2[x, l] = new Box();
                    int newCost = mod(dest_x - x + dest_y - l);
                    grid2[x, l].f = newCost;
                    if (newCost < cost)
                    {
                        cost = newCost;
                    }
                }
                if (x - 1 >= 0 && cost == grid2[x - 1, y].f)
                {
                    nextMove = "LEFT#";
                }
                else if (x + 1 <= 20 && cost == grid2[x + 1, y].f)
                {
                    nextMove = "RIGHT#";
                }
                else if (y - 1 >= 0 && cost == grid2[x, y - 1].f)
                {
                    nextMove = "UP#";
                }
                else if (y + 1 <= 20)
                {
                    nextMove = "DOWN#";
                }
                else
                {
                    nextMove = "DOWN#";
                }
            }
        }

        public int mod(int i)
        {
            if (i >= 0)
            {
                return i;
            }
            else
            {
                return (0 - i);
            }
        }
        public void updateObstacles()
        {
        }
        public void send(String aMessage)
        {
            this.client.send(aMessage);
        }

        public void createObstacles()
        {

            if (serverReply.ElementAt<char>(0) == 'I')
            {
                ///Console.WriteLine(serverReply.IndexOf('P'));
                //Console.WriteLine(serverReply.Length);
                String positions = serverReply.Substring(serverReply.IndexOf('P') + 3, (serverReply.Length - serverReply.IndexOf('P') - 3));
                ///Console.WriteLine(positions);
                ///String[] brickPositions = new String[3];
                String[] obstaclePositions = positions.Split(':');
                String type = "brickWall";
                for (int i = 0; i < obstaclePositions.Length; i++)
                {
                    
                    String[] posPairs = obstaclePositions[i].Split(';');
                    for (int j = 0; j < posPairs.Length;j++)
                    {
                        String[] pair = posPairs[j].Split(',');
                        Vector2 place = new Vector2(int.Parse(pair[0]), int.Parse(pair[1]));
                        obstacles.Add(new Obstacle(type, place));
                        grid[(int)place.X, (int)place.Y] = new MyPathNode()
                        {
                            IsWall = true,
                            X = (int)place.X,
                            Y = (int)place.Y,
                        };
                    }

                    if (i == 0)
                    {
                        type = "stoneWall";
                    }
                    else if (i == 1)
                    {
                        type = "water";
                    }
                    else
                    {

                    }
                    
                }
                for (int k = 0; k < 20; k++)
                {
                    for (int l = 0; l < 20; l++)
                    {
                        if (grid[k, l] == null)
                        {
                            grid[k, l] = new MyPathNode()
                            {
                                IsWall = false,
                                X = k,
                                Y = l,
                            };
                        }
                    }

                }
              
            }
            else
            {
            }

        }
        public void findNext()
        {
            int counter = 0;
            try
            {
                foreach (MyPathNode node in path)
                {
                    if (counter == 0)
                    {
                        counter++;
                        continue;
                    }
                    Vector2 p = new Vector2(node.X, node.Y);
                    if (p.X == position.X)
                    {
                        if (p.Y == position.Y - 1)
                        {
                            nextMove = "UP#";
                        }
                        else if (p.Y == position.Y + 1)
                        {
                            nextMove = "DOWN#";
                        }
                        else
                        {
                            Console.WriteLine("Wrong algo!!!!!!!!!!!!!!"+p.Y+"\t"+position.Y);
                        }

                    }
                    else if (p.Y == position.Y)
                    {
                        if (p.X == position.X - 1)
                        {
                            nextMove = "LEFT#";
                        }
                        else if (p.X == position.X + 1)
                        {
                            nextMove = "RIGHT#";
                        }
                        else
                        {
                            Console.WriteLine("Wrong algo!!!!!!!!!!!!!!" + p.X + "\t" + position.X);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong algo!!!!!!!!!!!!!!" + p.X + "\t" + position.X +"\t"+ p.Y  +"\t" + position.Y);
                    }
                    break;
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("No path found!!!");
                path = aStar.Search(new Vector2(position.X, position.Y), new Vector2(10,10), null, tanks[playerNum].direction);
                this.findNext();
            }
        }
      


        public void findPath()
        {
            aStar = new MySolver<MyPathNode, Object>(grid);
            try{
                if (tanks[playerNum].health < 70)
                {
                    if (lifePacks.ElementAt<LifePack>(0) != null)
                    {
                        //Console.WriteLine("No coin piles");
                        path = aStar.Search(new Vector2(position.X, position.Y), lifePacks, null, tanks[playerNum].direction);
                    }
                    else
                    {
                        path = aStar.Search(new Vector2(position.X, position.Y), coinPiles, null, tanks[playerNum].direction);
                    }
                }
                else{
                    if (coinPiles.ElementAt<Coins>(0) == null)
                    {
                        //Console.WriteLine("No coin piles");
                        path = aStar.Search(new Vector2(position.X, position.Y), new Vector2(10, 10), null, tanks[playerNum].direction);
                    }
                    else
                    {
                        //Console.WriteLine("coin piles found");
                        path = aStar.Search(new Vector2(position.X, position.Y), coinPiles, null, tanks[playerNum].direction);
                    }
                }
            }
            catch(ArgumentOutOfRangeException e){
               // Console.WriteLine("No coin piles");
                path = aStar.Search(new Vector2(position.X, position.Y), new Vector2(10, 10), null, tanks[playerNum].direction);
            }
            
        }
        IEnumerable<MyPathNode> path;
        MySolver<MyPathNode, Object> aStar;
        public void updateBullets()
        {
            
            try
            {
                List<Bullet> toBeRemoved = new List<Bullet>();
                foreach (Bullet bullet in bullets)
                {
                    Boolean bulletHit = false; 
                    if (bullet.direction == 0)
                    {
                        if (bullet.position.Y > 8)
                        {
                            bullet.position.Y = bullet.position.Y - 9;
                            if (!grid[(int)bullet.position.X / 30, (int)bullet.position.Y / 30].IsWalkable(5))
                            {
                                bulletHit = true;
                                bullet.hit = true;
                            }
                        }
                        else
                        {
                            bullet.hit=true;
                            bulletHit = true;
                        }
                    }
                    else if (bullet.direction == 1)
                    {
                        if (bullet.position.X <562)
                        {
                            bullet.position.X = bullet.position.X + 9;
                        }
                        else {
                            bullet.hit = true;
                            bulletHit = true;
                        }
                    }

                    else if (bullet.direction == 2)
                    {
                        if (bullet.position.Y < 562)
                        {
                            bullet.position.Y = bullet.position.Y + 9;
                        }
                        else
                        {
                            bullet.hit = true;
                            bulletHit = true;
                        }
                    }
                    else if (bullet.direction == 3)
                    {
                        if (bullet.position.X > 8)
                        {
                            bullet.position.X = bullet.position.X - 9;
                        }
                        else
                        {
                            bullet.hit = true;
                            bulletHit = true;
                        }
                    }
                    else
                    {
                    }
                    if (bulletHit == true)
                    {
                        toBeRemoved.Add(bullet);
                    }
                    //Console.WriteLine(bullet.position.X + "\t" + bullet.position.Y);
                }
                foreach (Bullet bullet in toBeRemoved){
                    bullets.Remove(bullet);
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
