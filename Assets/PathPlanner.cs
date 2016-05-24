using UnityEngine;
using System.Collections;

using System.Collections.Generic;

static public class PathPlanner
{
    static public List<Solution> PreCalculatedSolutions = new List<Solution>();

    public class Solution : System.IEquatable<Solution>
    {
        public Tile Start
        {
            get;
            private set;
        }
        public Tile End
        {
            get;
            private set;
        }

        public List<Tile> Path = new List<Tile>();

        public Solution(Tile _Start, Tile _End)
        {
            Start = _Start;
            End = _End;
        }

        public bool Equals(Solution other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }
    }
	
    public class Task<T>
    {
        public T ReturnValue
        {
            get;
            set;
        }

        public bool Finished
        {
            get;
            set;
        }

        public System.Threading.Thread TaskThread
        {
            get;
            set;
        }
    }

    static public Task<Solution> SolveAsync(Tile Start, Tile Goal, IEvaluation Eval)
    {
        Solution Solved = PreCalculatedSolutions.Find(S => S.Start.Pos == Start.Pos && S.End.Pos == Goal.Pos);
        Task<Solution> AwaitTask = new Task<Solution>();

        if(Solved != null)
        {
            AwaitTask.TaskThread = new System.Threading.Thread(new System.Threading.ThreadStart(
                () => 
                    AStarAsync(Start, Goal, Eval, AwaitTask)
                    ));

            AwaitTask.TaskThread.Start();
        }
        else
        {
            AwaitTask.Finished = true;
            AwaitTask.ReturnValue = Solved;
        }

        return AwaitTask;
    }

    static void AStarAsync(Tile Start, Tile Goal, IEvaluation Eval, Task<Solution> SolveTask)
    {
        Solution Solved = SolveASTAR(Start, Goal, Eval);

        SolveTask.ReturnValue = Solved;
        SolveTask.Finished = true;
    }

    public interface IEvaluation
    {
        float Evaluate(Tile CurPosition, Tile Goal);
    }

    public class ManhattanHueristic : IEvaluation
    {
        public float Evaluate(Tile CurPosition, Tile Goal)
        {
            return Mathf.Abs(CurPosition.Pos.x - Goal.Pos.x) + Mathf.Abs(CurPosition.Pos.y - Goal.Pos.y);
        }
    }

    public class ChebyshevHueristic : IEvaluation
    {
        public float Evaluate(Tile CurPosition, Tile Goal)
        {
            return Mathf.Max(Mathf.Abs(CurPosition.Pos.x - Goal.Pos.x), Mathf.Abs(CurPosition.Pos.y - Goal.Pos.y));
        }
    }

    static public ManhattanHueristic ManhattanHueristicEvaluation = new ManhattanHueristic();
    static public ChebyshevHueristic ChebyshevHueristicEvaluation = new ChebyshevHueristic();
    

    static public Solution SolveImmediate(Tile Start, Tile Goal, IEvaluation Eval)
    {
        Solution Solved = PreCalculatedSolutions.Find(S => S.Start.Pos == Start.Pos && S.End.Pos == Goal.Pos);

        if(Solved == null)
        {
            Solved = SolveASTAR(Start, Goal, Eval);

            PreCalculatedSolutions.Add(Solved);
        }

        return Solved;
    }

    class AStarNode<T> : System.IComparable<AStarNode<T>>, System.IEquatable<AStarNode<T>> where T:System.IEquatable<T>
    {
        public AStarNode<T> Parent
        {
            get;
            private set;
        }

        public T Node
        {
            get;
            private set;
        }

        public float G
        {
            get;
            private set;
        }

        public float H
        {
            get;
            private set;
        }

        public float F
        {
            get
            {
                return G + H;
            }
        }

        public AStarNode(AStarNode<T> _Parent, T _t, float g, float h)
        {
            Parent = _Parent;
            Node = _t;
            G = g;
            H = h;
        }

        public int CompareTo(AStarNode<T> other)
        {
            return F.CompareTo(other.F);
        }

        public bool Equals(AStarNode<T> other)
        {
            return Node.Equals(other.Node);
        }
    }

    static public Solution SolveASTAR(Tile Start, Tile Goal, IEvaluation Eval)
    {
        Solution Solved = new Solution(Start, Goal);

        List<AStarNode<Tile>> Open = new List<AStarNode<Tile>>();
        List<AStarNode<Tile>> Closed = new List<AStarNode<Tile>>();

        AStarNode<Tile> TerminalNode = null;

        Open.InsertSorted(new AStarNode<Tile>(null, Start, 0.0f, Eval.Evaluate(Start, Goal)));

        while(Open.Count > 0)
        {
            AStarNode<Tile> Q = Open[0];
            Open.Remove(Q);

            if(Q.Node.Equals(Goal))
            {
                TerminalNode = Q;
                break;
            }

            foreach(Tile t in Q.Node.AccessibleSurroundingTiles())
            {
                AStarNode<Tile> N = new AStarNode<Tile>(Q, t, Q.G + 1.0f, Eval.Evaluate(t, Goal));

                if(!Closed.Contains(N))
                {
                    AStarNode<Tile> AlreadyExploredOpen = Open.Find(S => S.Equals(N));
                    if(AlreadyExploredOpen != null)
                    {
                        if(AlreadyExploredOpen.F > N.F)
                        {
                            Open.Remove(AlreadyExploredOpen);
                            Open.InsertSorted(N);
                        }
                    }
                    else
                    {
                        Open.InsertSorted(N);
                    }
                }
            }

            Closed.Add(Q);
        }

        AStarNode<Tile> Cur = TerminalNode;
        while(Cur.Parent != null)
        {
            Solved.Path.Insert(0, Cur.Node);
            Cur = Cur.Parent;
        }

        return Solved;
    }

    public static void InsertSorted<T>(this List<T> OrderedList, T t) where T : System.IComparable<T>
    {
        int i = 0;
        for (i = 0; i != OrderedList.Count; ++i)
        {
            if ((OrderedList[i].CompareTo(t) == 1) || (i == OrderedList.Count - 1))
            {
                break;
            }
        }
        OrderedList.Insert(i, t);
    }
}
