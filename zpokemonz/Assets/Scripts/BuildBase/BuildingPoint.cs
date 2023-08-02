public class BuildingPoint
{
    public int X { get; set; }
    public int Y { get; set; }

    public BuildingPoint(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    //检查相同点
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is BuildingPoint)
        {
            BuildingPoint p = obj as BuildingPoint;
            return this.X == p.X && this.Y == p.Y;
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 6949;
            hash = hash * 7907 + X.GetHashCode();
            hash = hash * 7907 + Y.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return "P(" + this.X + ", " + this.Y + ")";
    }
}