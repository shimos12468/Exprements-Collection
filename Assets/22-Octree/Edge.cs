namespace Octree
{
    public class Edge
	{
		public readonly Node a , b;
		public Edge(Node a ,Node b)
		{
			this.a = a;
			this.b = b;
		}

        public override bool Equals(object obj)
        {
            return obj is Edge other && ((other.a==a &&other.b==b)|| (other.a == b && other.b == a)) ;
        }
        public override int GetHashCode()
        {
            return a.GetHashCode()^b.GetHashCode();
        }

    }
}