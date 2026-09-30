using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp_SproutInAGarden.GameObjects;


namespace MauiApp_SproutInAGarden.GameLogic
{
    public class FakeStream : InputOutputStream
    {
        public string ReadLine()
        {
            return "";
        }
        public void WriteLine(string line)
        {

        }
    }

    public class Plant
    {
        public InputOutputStream stream;
        public part_of_a_plant root;
        public PlantParams Gens;

        public Random rand;
        public int tick;
        public Vec2d sun;
       // public static Bitmap bmp;
       // public static System.Windows.Forms.PictureBox thisPictureBox;
       // public static Pen pen;

        public Plant() : this(new PlantParams() 
        {
            gr = new PP_growing() 
            {
                MaxLen = 12,
                DyingOff = 8,
                Youth = 6,
                Branches = 1,

                StepMaxLen = 80,
                StepMinLen = 50,

                Bushiness = 0.10,
                Vegetation = 0.45,
                Slimness = 0.70,
                Fade = 0.01,
                CurlyMax = 0.50,
                CurlyMin = 0.20,
                Weight = 0.20,
                Fall = 0.01,
                Plasticity = 0.08,
                Deviation = 0.10
                /*
                 * Bushiness =
                 * Vegetation 
                 * Slimness = 
                 * Fade = GetF
                 * CurlyMin = 
                 * CurlyMax = 
                 * 
                 * Weight = Ge
                 * Fall = GetF
                 * Plasticity 
                 * Deviation =
                 * 
                 * 
                 * 
                 */
            },
            visual = new PP_visual() 
            {
                StepX = 0, 
                StepY = 0, 
                TickPerClick = 10
            }
        })
        {
            
        }

        public Plant(/*System.Windows.Forms.PictureBox thisPictureBox,*/ PlantParams p)
        {
            Gens = p;

            //Plant.thisPictureBox = thisPictureBox;
            sun = new Vec2d(200, -5000);
            rand = new Random();
            //bmp = new Bitmap(thisPictureBox.Width, thisPictureBox.Height);

            root = new part_of_a_plant(new Vec2d(0, -80), this);
        }

        public void connect(InputOutputStream stream)
        {
            this.stream = stream;
            stream.WriteLine("{Plant} - connected to stream");
        }

        public bool RandomChance(double percent)
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
        public part_of_a_plant drowPlant()
        {
            return root;
        }
    }
    public static class TagsList
    {
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

        public static void Init()
        {
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
    public class part_of_a_plant : /* Plant,*/ TagableType
    {
        public Plant BasePlant { get; }
        part_of_a_plant mainKid;
        List<part_of_a_plant> kids;
        public int old;
        public string status;   // none active passive dead
        public string ID;       // 0-9 - main, a-z - slave
        public Vec2d end;
        public PlantTagManager myTags;
        public BaseGameObject my_obj;
        public part_of_a_plant(Vec2d parent_end, Plant basePlant)
        {
            BasePlant = basePlant;
            old = 0;
            myTags = new PlantTagManager();
            myTags.Add(TagsList.tag_none);
            end = new Vec2d(parent_end);
        }

        public void born(string ID)
        {
            if (myTags.Contains(TagsList.tag_master))
            {
                double curly = BasePlant.Gens.gr.Deviation * BasePlant.rand.NextDouble();
                double side = end.GetSide(BasePlant.sun);
                double slimnessFactor = BasePlant.RandomChance(BasePlant.Gens.gr.Slimness) ? side : -side;
                end.RotateVector(curly * 140 * slimnessFactor);
            }
            else if (myTags.Contains(TagsList.tag_slave))
            {
                double curly = BasePlant.Gens.gr.CurlyMin + (BasePlant.Gens.gr.CurlyMax - BasePlant.Gens.gr.CurlyMin) * BasePlant.rand.NextDouble();
                double side = end.GetSide(BasePlant.sun);
                double slimnessFactor = BasePlant.RandomChance(BasePlant.Gens.gr.Slimness) ? side : -side;
                end.RotateVector(curly * 140 * slimnessFactor);
            }

            end = end.Normalize();
            end = end * (BasePlant.Gens.gr.StepMinLen + (BasePlant.Gens.gr.StepMaxLen - BasePlant.Gens.gr.StepMinLen) * BasePlant.rand.NextDouble());

            this.ID = ID;
            kids = new List<part_of_a_plant>();
            mainKid = new part_of_a_plant(new Vec2d(end), BasePlant);

            mainKid.myTags.Switch(TagsList.tag_master, TagsList.priority);

            myTags.Switch(TagsList.tag_active, TagsList.phase);
        }
        public void Fall(int len)
        {
            double t = (double)(len - 1) / (BasePlant.Gens.gr.MaxLen - 1);
            double exponent = 1.0 / (BasePlant.Gens.gr.Weight * 1.2 + 0.4);
            double curve = Math.Pow(t, exponent);
            double value = -BasePlant.Gens.gr.Plasticity + curve * (BasePlant.Gens.gr.Plasticity * 0.55 + BasePlant.Gens.gr.Plasticity);
            value = Math.Round(value, 3);

            double side = end.GetSide(BasePlant.sun);

            end.RotateVector(side * 140 * value);

        }
        public void die()
        {
            if (myTags.Contains(TagsList.tag_none))
                return;

            if (mainKid != null)
                mainKid.die();

            if (kids != null)
            {
                foreach (var kid in kids)
                    kid.die();
            }

            myTags.Switch(TagsList.tag_dead, TagsList.phase);


            if (myTags.Contains(TagsList.tag_dead))
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

            if (!myTags.Contains(TagsList.tag_none))
            {

                my_obj = new BaseGameObject(myDot, parentDot);

                if (mainKid != null)
                    mainKid.drow(myDot);

                if (kids != null)
                {
                    foreach (var kid in kids)
                        kid.drow(myDot);
                }
            }
        }
        public void drowFlash(SKCanvas canvas)
        {

            if (!myTags.Contains(TagsList.tag_none))
            {
                my_obj.Draw(canvas);

                if (mainKid != null)
                    mainKid.drowFlash(canvas);

                if (kids != null)
                {
                    foreach (var kid in kids)
                        kid.drowFlash(canvas);
                }
            }
        }

        public void oneTick(int len)
        {
            if ((len == BasePlant.Gens.gr.MaxLen) || myTags.Contains(TagsList.tag_none) || myTags.Contains(TagsList.tag_dead))
                return;

            len++;
            old++;

            if (!myTags.Contains(TagsList.tag_dead))
            {
                if (old == BasePlant.Gens.gr.Youth)
                    myTags.Switch(TagsList.tag_passive, TagsList.phase);

                if (old == BasePlant.Gens.gr.DyingOff)
                    myTags.Switch(TagsList.tag_faded, TagsList.phase);
            }


            if (len == BasePlant.Gens.gr.MaxLen)
            {
                //stream.WriteLine($"branch {ID} is finished");
            }
            else
            {
                if (myTags.Contains(TagsList.tag_active))
                {
                    if (mainKid != null)
                        mainKid.oneTick(len);

                    if (kids != null)
                    {
                        foreach (var kid in kids)
                            kid.oneTick(len);
                    }

                    if (BasePlant.RandomChance(BasePlant.Gens.gr.Vegetation) && (mainKid != null && mainKid.myTags.Contains(TagsList.tag_none)))
                    {
                        mainKid.born(ID + "A");
                    }

                    if (BasePlant.RandomChance(BasePlant.Gens.gr.Bushiness) && (kids != null && kids.Count < BasePlant.Gens.gr.Branches))
                    {
                        part_of_a_plant newKid = new part_of_a_plant(end, BasePlant);

                        newKid.myTags.Switch(TagsList.tag_slave, TagsList.priority);
                        newKid.myTags.Switch(TagsList.tag_active, TagsList.phase);

                        char lastChar = ID[ID.Length - 1];
                        newKid.born(ID + (char)(lastChar + 1));
                        kids.Add(newKid);
                    }
                    if (BasePlant.RandomChance(BasePlant.Gens.gr.Fall))
                    {
                        Apply(TagsList.tag_fallen);
                    }
                }

                if (myTags.Contains(TagsList.tag_passive) || myTags.Contains(TagsList.tag_faded))
                {
                    if (mainKid != null)
                        mainKid.oneTick(len);

                    if (kids != null)
                    {
                        foreach (var kid in kids)
                            kid.oneTick(len);

                        if (myTags.Contains(TagsList.tag_faded))
                            foreach (var kid in kids)
                                if (BasePlant.RandomChance(BasePlant.Gens.gr.Fade))
                                    kid.die();
                    }
                }
                if (myTags.Contains(TagsList.tag_fallen))
                {
                    if ((end.Normalize() * new Vec2d(0, -1) >= 0.97) || (end.Normalize() * new Vec2d(0, -1) <= -0.97))
                    {
                        Apply(TagsList.tag_fallen, false);
                        Apply(TagsList.tag_fixed);
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


}
