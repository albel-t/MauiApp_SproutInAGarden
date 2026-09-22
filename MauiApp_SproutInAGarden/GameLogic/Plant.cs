using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MauiApp_SproutInAGarden.GameLogic
{
    public class Plant
    {
        public static InputOutputStream stream;
        public part_of_a_plant root;
        public static PlantParams Gens;

        public static Random rand;
        public static int tick;
        public static Vec2d sun;
       // public static Bitmap bmp;
       // public static System.Windows.Forms.PictureBox thisPictureBox;
       // public static Pen pen;

        public static PlantTag tag_none;
        public static PlantTag tag_active;
        public static PlantTag tag_passive;
        public static PlantTag tag_faded;
        public static PlantTag tag_dead;
        public static PlantTag tag_original;
        public static PlantTag tag_fallen;
        public static PlantTag tag_supported;
        public static PlantTag tag_master;
        public static PlantTag tag_slave;
        public static PlantTag tag_fixed;

        public static PlantTagGroup phase;
        public static PlantTagGroup position;
        public static PlantTagGroup priority;


        public Plant()
        { }

        public Plant(/*System.Windows.Forms.PictureBox thisPictureBox,*/ PlantParams p)
        {
            Gens = p;

            //Plant.thisPictureBox = thisPictureBox;
            sun = new Vec2d(200, -5000);
            rand = new Random();
            //bmp = new Bitmap(thisPictureBox.Width, thisPictureBox.Height);

            root = new part_of_a_plant(new Vec2d(0, -30));

            tag_none = new PlantTag("none");
            tag_active = new PlantTag("active");
            tag_passive = new PlantTag("passive");
            tag_faded = new PlantTag("faded");
            tag_dead = new PlantTag("dead");

            tag_original = new PlantTag("original");
            tag_fallen = new PlantTag("fallen");
            tag_supported = new PlantTag("supported");
            tag_master = new PlantTag("master");
            tag_slave = new PlantTag("slave");
            tag_fixed = new PlantTag("fixed");

            phase = new PlantTagGroup("phase");
            phase.Add(tag_none);
            phase.Add(tag_active);
            phase.Add(tag_passive);
            phase.Add(tag_faded);
            phase.Add(tag_dead);

            position = new PlantTagGroup("position");
            position.Add(tag_original);
            position.Add(tag_fallen);
            position.Add(tag_supported);

            priority = new PlantTagGroup("priority");
            priority.Add(tag_master);
            priority.Add(tag_slave);
            priority.Add(tag_fixed);

        }

        public static void connect(InputOutputStream stream)
        {
            Plant.stream = stream;
            stream.WriteLine("{Plant} - connected to stream");
        }

        public static bool RandomChance(double percent)
        {
            double randomValue = rand.NextDouble();
            return randomValue < percent;
        }

        public void tickPlant()
        {
            root.oneTick(1);
        }

        public void firatTickPlant()
        {
            root.born("A");
            stream.WriteLine("root was born");
        }
        public void drowPlant()
        {
            
        }

        public static void DrawLine(Vec2d start, Vec2d stop, int color, double width = 0)
        {
            
        }
        /*

        public void drowPlant()
        {
            stream.WriteLine($"tick {root.old}");
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
            }
            root.drow(new Vec2d(Gens.visual.StepX, Gens.visual.StepY));
            thisPictureBox.Image = bmp;
        }

        public static void DrawLine(Vec2d start, Vec2d stop, int color, double width = 0)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                switch (color)
                {
                    case 0:
                        pen = new Pen(Color.LightGreen, (float)(3 + width));
                        break;
                    case 1:
                        pen = new Pen(Color.Green, (float)(3 + width));
                        break;
                    case 2:
                        pen = new Pen(Color.Brown, (float)(3 + width));
                        break;
                }
                g.DrawLine(pen, (float)start.x, (float)start.y, (float)stop.x, (float)stop.y);
            }
        }
        */
    }
    public struct PlantParams
    {
        public PP_visual visual;
        public PP_growing gr;
        public override string ToString()
        {
            return $"Visual: [{visual}], \nGrowing: [{gr}]";
        }
    }
    public struct PP_visual
    {
        public int StepX, StepY, TickPerClick;
        public override string ToString()
        {
            return $"StepX: {StepX}, StepY: {StepY}, TickPerClick:{TickPerClick}";
        }
    }
    public struct PP_growing
    {
        public int MaxLen, DyingOff, Youth, Branches;
        public int StepMaxLen, StepMinLen;
        public double Bushiness, Vegetation, Slimness, Fade, CurlyMax, CurlyMin, Weight, Fall, Plasticity, Deviation;
        public override string ToString()
        {
            return $"MaxLen: {MaxLen},\n DyingOff: {DyingOff},\n Youth: {Youth},\n Branches: {Branches},\n - \n " +
                   $"StepMaxLen: {StepMaxLen},\n StepMinLen: {StepMinLen},\n - \n " +
                   $"Bushiness: {Bushiness:F2},\n Vegetation: {Vegetation:F2},\n Slimness: {Slimness:F2},\n " +
                   $"Fade: {Fade:F2},\n CurlyMin: {CurlyMin:F2},\n CurlyMax: {CurlyMax:F2}, \n " +
                   $"Weight: {Weight:F2},\n Fall: {Fall:F3},\n Plasticity: {Plasticity:F2},\n Deviation: {Deviation:F2}";
        }
    }
    public interface InputOutputStream
    {
        string ReadLine();
        void WriteLine(string line);
    }
    public class part_of_a_plant : Plant, TagableType
    {
        part_of_a_plant mainKid;
        List<part_of_a_plant> kids;
        public int old;
        public string status;   // none active passive dead
        public string ID;       // 0-9 - main, a-z - slave
        public Vec2d end;
        public PlantTagManager myTags;
        public part_of_a_plant(Vec2d parent_end) : base()
        {
            old = 0;
            myTags = new PlantTagManager();
            myTags.Add(tag_none);
            end = new Vec2d(parent_end);
        }

        public void born(string ID)
        {
            if (myTags.Contains(tag_master))
            {
                double curly = Gens.gr.Deviation * rand.NextDouble();
                double side = end.GetSide(sun);
                double slimnessFactor = RandomChance(Gens.gr.Slimness) ? side : -side;
                end.RotateVector(curly * 140 * slimnessFactor);
            }
            else if (myTags.Contains(tag_slave))
            {
                double curly = Gens.gr.CurlyMin + (Gens.gr.CurlyMax - Gens.gr.CurlyMin) * rand.NextDouble();
                double side = end.GetSide(sun);
                double slimnessFactor = RandomChance(Gens.gr.Slimness) ? side : -side;
                end.RotateVector(curly * 140 * slimnessFactor);
            }

            end = end.Normalize();
            end = end * (Gens.gr.StepMinLen + (Gens.gr.StepMaxLen - Gens.gr.StepMinLen) * rand.NextDouble());

            this.ID = ID;
            kids = new List<part_of_a_plant>();
            mainKid = new part_of_a_plant(new Vec2d(end));

            mainKid.myTags.Switch(tag_master, priority);

            myTags.Switch(tag_active, phase);
        }
        public void Fall(int len)
        {
            double t = (double)(len - 1) / (Gens.gr.MaxLen - 1);
            double exponent = 1.0 / (Gens.gr.Weight * 1.2 + 0.4);
            double curve = Math.Pow(t, exponent);
            double value = -Gens.gr.Plasticity + curve * (Gens.gr.Plasticity * 0.55 + Gens.gr.Plasticity);
            value = Math.Round(value, 3);

            double side = end.GetSide(sun);

            end.RotateVector(side * 140 * value);

        }
        public void die()
        {
            if (myTags.Contains(tag_none))
                return;

            if (mainKid != null)
                mainKid.die();

            if (kids != null)
            {
                foreach (var kid in kids)
                    kid.die();
            }

            myTags.Switch(tag_dead, phase);


            if (myTags.Contains(tag_dead))
            {
                //stream.WriteLine($"branch {ID} is dead");
            }
        }
        public void Apply(PlantTag GlobalTag, bool AddDel = true)
        {
            if (AddDel) // add
            {
                myTags.Add(GlobalTag);
            }
            else//del
            {
                myTags.Delete(GlobalTag);
            }
            if (kids != null)
            {
                mainKid.Apply(GlobalTag, AddDel);
                foreach (var kid in kids)
                    kid.Apply(GlobalTag, AddDel);
            }
        }
        public void drow(Vec2d parentDot)
        {
            Vec2d myDot = parentDot + (end + Math.Pow(old, 0.75));

            if (!myTags.Contains(tag_none))
            {
                DrawLine(parentDot, myDot, myTags.Contains(tag_active) ? 0 : ((myTags.Contains(tag_passive) || myTags.Contains(tag_faded)) ? 1 : 2), Math.Sqrt(old));

                if (mainKid != null)
                    mainKid.drow(myDot);

                if (kids != null)
                {
                    foreach (var kid in kids)
                        kid.drow(myDot);
                }
            }
        }

        public void oneTick(int len)
        {
            if ((len == Gens.gr.MaxLen) || myTags.Contains(tag_none) || myTags.Contains(tag_dead))
                return;

            len++;
            old++;

            if (!myTags.Contains(tag_dead))
            {
                if (old == Gens.gr.Youth)
                    myTags.Switch(tag_passive, phase);

                if (old == Gens.gr.DyingOff)
                    myTags.Switch(tag_faded, phase);
            }


            if (len == Gens.gr.MaxLen)
            {
                //stream.WriteLine($"branch {ID} is finished");
            }
            else
            {
                if (myTags.Contains(tag_active))
                {
                    if (mainKid != null)
                        mainKid.oneTick(len);

                    if (kids != null)
                    {
                        foreach (var kid in kids)
                            kid.oneTick(len);
                    }

                    if (RandomChance(Gens.gr.Vegetation) && (mainKid != null && mainKid.myTags.Contains(tag_none)))
                    {
                        mainKid.born(ID + "A");
                    }

                    if (RandomChance(Gens.gr.Bushiness) && (kids != null && kids.Count < Gens.gr.Branches))
                    {
                        part_of_a_plant newKid = new part_of_a_plant(end);

                        newKid.myTags.Switch(tag_slave, priority);
                        newKid.myTags.Switch(tag_active, phase);

                        char lastChar = ID[ID.Length - 1];
                        newKid.born(ID + (char)(lastChar + 1));
                        kids.Add(newKid);
                    }
                    if (RandomChance(Gens.gr.Fall))
                    {
                        Apply(tag_fallen);
                    }
                }

                if (myTags.Contains(tag_passive) || myTags.Contains(tag_faded))
                {
                    if (mainKid != null)
                        mainKid.oneTick(len);

                    if (kids != null)
                    {
                        foreach (var kid in kids)
                            kid.oneTick(len);

                        if (myTags.Contains(tag_faded))
                            foreach (var kid in kids)
                                if (RandomChance(Gens.gr.Fade))
                                    kid.die();
                    }
                }
                if (myTags.Contains(tag_fallen))
                {
                    if ((end.Normalize() * new Vec2d(0, -1) >= 0.97) || (end.Normalize() * new Vec2d(0, -1) <= -0.97))
                    {
                        Apply(tag_fallen, false);
                        Apply(tag_fixed);
                    }
                    else
                    {
                        Fall(len);
                    }
                }
            }
        }
    }

    public interface TagableType
    {
        void Apply(PlantTag GlobalTag, bool AddDel = true);
    }

    public struct PlantTagGroup
    {
        public string Group;
        public List<PlantTag> tags;
        public PlantTagGroup(string group)
        {
            this.Group = group;
            tags = new List<PlantTag>();
        }
        public PlantTagGroup(PlantTagGroup other)
        {
            tags = new List<PlantTag>(other.tags);
            Group = other.Group;
        }
        public void Add(PlantTag newTag)
        {
            if (!tags.Contains(newTag))
            {
                tags.Add(newTag);
            }
        }
        public void Import(string group, PlantTagManager other)
        {
            tags = new List<PlantTag>(other.tags);
            Group = group;
        }
    }

    public struct PlantTag
    {
        public string Name;
        public PlantTag(string name)
        {
            Name = name;
        }
        public override bool Equals(object obj)
        {
            if (obj is PlantTag other)
            {
                return Name == other.Name;
            }
            return false;
        }
    }

    public class PlantTagManager
    {
        public List<PlantTag> tags;

        public PlantTagManager()
        {
            tags = new List<PlantTag>();
        }
        public PlantTagManager(PlantTagManager other)
        {
            tags = new List<PlantTag>(other.tags);
        }

        public void Add(PlantTag newTag)
        {
            if (!tags.Contains(newTag))
            {
                tags.Add(newTag);
            }
        }

        public void Delete(PlantTag newTag)
        {
            if (tags.Contains(newTag))
            {
                tags.Remove(newTag);
            }
        }
        public void Switch(PlantTag newTag, PlantTagGroup newTags)
        {
            Delete(newTags);
            Add(newTag);
        }
        public void Delete(PlantTagGroup newTags)
        {
            foreach (var newTag in newTags.tags)
            {
                if (tags.Contains(newTag))
                {
                    tags.Remove(newTag);
                }
            }
        }

        public void Remove(PlantTag tag)
        {
            tags.Remove(tag);
        }

        public bool Contains(PlantTag tag)
        {
            return tags.Contains(tag);
        }

        public void Clear()
        {
            tags.Clear();
        }

        public int Count => tags.Count;
    }

    public class Vec2d
    {
        public double x, y;
        public Vec2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public Vec2d(Vec2d other)
        {
            this.x = other.x;
            this.y = other.y;
        }
        public override string ToString()
        {
            return $"({x}, {y})";
        }
        public double Length()
        {
            return Math.Sqrt(x * x + y * y);
        }
        public Vec2d Normalize()
        {
            double len = Length();
            if (len == 0) return new Vec2d(0, 0);
            return new Vec2d(x / len, y / len);
        }
        public void Vec2dPlant(double angle, double len)
        {
            double angleRadians = angle * Math.PI / 180.0;
            x = Math.Sin(angleRadians) * len;
            y = -Math.Cos(angleRadians) * len;

        }
        public void RotateVector(double angle)
        {
            double length = Math.Sqrt(x * x + y * y);
            double currentAngleRad = Math.Atan2(x, -y);
            double newAngleRad = currentAngleRad + (angle * Math.PI / 180.0);

            x = Math.Sin(newAngleRad) * length;
            y = -Math.Cos(newAngleRad) * length;
        }
        public int GetSide(Vec2d p2)
        {
            double cross = x * p2.y - y * p2.x;

            if (cross < 0)
                return -1;
            else
                return 1;
        }

        public static Vec2d operator +(Vec2d a, Vec2d b)
        {
            return new Vec2d(a.x + b.x, a.y + b.y);
        }

        public static Vec2d operator -(Vec2d a, Vec2d b)
        {
            return new Vec2d(a.x - b.x, a.y - b.y);
        }

        public static Vec2d operator *(Vec2d v, double scalar)
        {
            return new Vec2d(v.x * scalar, v.y * scalar);
        }

        public static Vec2d operator *(double scalar, Vec2d v)
        {
            return new Vec2d(v.x * scalar, v.y * scalar);
        }
        public static double operator *(Vec2d a, Vec2d b)
        {
            return a.x * b.x + a.y * b.y;
        }
        public static Vec2d operator /(Vec2d v, double scalar)
        {
            return new Vec2d(v.x / scalar, v.y / scalar);
        }
        public static Vec2d operator +(Vec2d v, double scalar)
        {
            if (v == null) return null;

            double len = v.Length();
            if (len == 0) return new Vec2d(0, 0);

            double newLen = len + scalar;
            double scale = newLen / len;

            return new Vec2d(v.x * scale, v.y * scale);
        }
        public static Vec2d operator +(double scalar, Vec2d v)
        {
            return v + scalar;
        }
    }

}
