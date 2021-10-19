using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lab1V1
{
    static class VectorMethods
    {
        public static double ComplexModule(double real, double image)
        {
            return Math.Sqrt(Math.Pow(real, 2) + Math.Pow(image, 2));
        }
    }

    struct DataItem
    {
        public double x { get; set; }
	public double y { get; set; }
        public Complex value { get; set; }
        public DataItem(double x, double y, Complex value)
        {
            this.x = x;
            this.y = y;
	    this.value = value;
        }
        public string ToLongString(string format)
        {
            return String.Format(format, x, y, value.Real, value.Imaginary, 
				    VectorMethods.ComplexModule(value.Real, value.Imaginary));
        }
        public override string ToString()
        {
            return ToLongString("x {0:f2} y {1:f2} xValue {2:f2} yValue {3:f2} module {4:f2}");
        }
    }

    delegate Complex FdlbComplex(double x, double y);

    abstract class V1Data
    {
        public string object_id { get; }
        public DateTime data { get; }

        public V1Data(string object_id, DateTime data)
        {
            this.object_id = object_id;
            this.data = data;
        }
        public abstract int Count { get; }
        public abstract double AverageValue { get; }
        public abstract string ToLongString(string format);
        public abstract override string ToString();
    }

    class V1DataList : V1Data
    {
        public List<DataItem> DataList { get; }
        public V1DataList(string object_id, DateTime data) : base(object_id, data)
        {
            DataList = new List<DataItem>();
        }
        public bool Add(DataItem newItem)
        {
            foreach (DataItem Item in DataList)
            {
                if (Item.x == newItem.y && Item.x == newItem.y)
                {
                    return false;
                }
            }
            DataList.Add(newItem);
            return true;
        }

        public int AddDefaults(int nItems, FdlbComplex F)
        {
            int count = 0;
            for (int i = 0; i < nItems; i++)
            {
                double x = i * 1.42;
                double y = i * 1.11;
                DataItem newItem = new DataItem(x, y, F(x, y));
                if (Add(newItem)) 
                {
                    count++;
                }
            }
            return count;
        }

        public override int Count
        {
            get { return DataList.Count; }
        }

        public override double AverageValue
        {
            get
            {
                if (Count == 0) {
                    return 0;
                }
                double averageValue = 0;
                foreach (DataItem item in DataList)
                {
		    averageValue += VectorMethods.ComplexModule(item.value.Real, item.value.Imaginary);
                }
                return averageValue / Count;
            }
        }

        public override string ToLongString(string format)
        {
            string str1 = String.Format(" Class:{0}\n object_id:{1}\n Count:{2}\n",
					    "V1DataList", object_id, Count) + '\n';
            string str2 = "";
            foreach (DataItem item in DataList)
            {
                str2 += string.Format(format, item.x, item.y, item.value.Real, item.value.Imaginary,
					  VectorMethods.ComplexModule(item.value.Real, item.value.Imaginary));
            }
            return str1 + str2;
        }
        public override string ToString()
        {
            return String.Format(" Class:{0}\n object_id:{1}\n Count:{2}\n", 
				    "V1DataList", object_id, Count) + '\n';
        }

    }


    class V1DataArray : V1Data
    {
	public int xNodes { get; }
	public int yNodes { get; }
        public double xSteps { get; }
        public double ySteps { get; }
        public Complex Step { get; }
        public Complex[,] array { get; }

        public V1DataArray(string object_id, DateTime data) : base(object_id, data)
        {
            array = new Complex[0, 0];
        }
        public V1DataArray(string object_id, DateTime data, int xNodes, int yNodes, 
				double xSteps, double ySteps, FdlbComplex F) : base(object_id, data)
        {
	    this.xNodes = xNodes;
	    this.yNodes = yNodes;
            this.xSteps = xSteps;
            this.ySteps = ySteps;
            array = new Complex[xNodes, yNodes];
            for (int i = 0; i < xNodes; i++)
            {
                for (int j = 0; j < yNodes; j++)
                {
                    array[i, j] = F(i * xSteps, j * ySteps);
                }
            }
        }
        public override int Count
        {
            get
            {
                return xNodes * yNodes;
            }
        }
				
	public override double AverageValue
        {
            get
            {
                double averageValue = 0; 
                for (int i = 0; i < xNodes; i++)
                {
                    for (int j = 0; j < yNodes; j++)
                    {
		        averageValue += VectorMethods.ComplexModule(array[i, j].Real, array[i, j].Imaginary); 
		    }
                }

                return averageValue / Count;
            }
        }

        public override string ToString()
        {
            return String.Format(" Class:{0}\n object_id:{1}\n xNodes:{2} yNodes:{3} xSteps:{4} ySteps:{5} Module\n", 
				    "V1DataArray", object_id, xNodes, yNodes, xSteps, ySteps) + '\n';
        }
				
	public override string ToLongString(string format)
        {
            string str1 = String.Format(" Class:{0}\n object_id:{1}\n xNodes:{2} yNodes:{3} xSteps:{4} ySteps:{5} Module\n", 
					    "V1DataArray", object_id, xNodes, yNodes, xSteps, ySteps) + '\n';
            string str2 = "";
            for (int i = 0; i < xNodes; i++)
            {
                for (int j = 0; j < yNodes; j++)
                {
                    str2 += String.Format(format, i * xNodes, j * yNodes, array[i,j].Real, 
					      array[i,j].Imaginary, VectorMethods.ComplexModule(array[i,j].Real, array[i, j].Imaginary));
                }
            }	
	    str1 += str2;
            return str1;
        }

        public V1DataList ArrayToList()
        {
            V1DataList DataList = new V1DataList(object_id, data);
            for (int i = 0; i < xNodes; i++)
            {
                for (int j = 0; j < yNodes; j++)
                {
                    double x = i * xNodes;
		    double y = j * yNodes;
                    Complex value = array[i, j];
                    DataItem item = new DataItem(x, y, value);
                    DataList.Add(item);
                }
            }
            return DataList;
        }
    }

    class V1MainCollection
    {
        private List<V1Data> Collection = new List<V1Data>();
        public int Count()
        {
            return Collection.Count;
        }
        public V1Data this[int index]
        {
            get
            {
                return Collection[index];
            }
        }
        public bool Contains(string ID)
        {
            foreach (V1Data Data in Collection)
            {
                if (Data.object_id == ID)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Add (V1Data v1Data)
        {
            if (!Contains(v1Data.object_id))
            {
                Collection.Add(v1Data);
                return true;
            }
            return false;
        }
        public string ToLongString(string format)
        {
            string str1 = "";
            foreach (V1Data item in Collection)
            {
                str1 += item.ToLongString(format);
            }
            return str1;
        }
				
	public override string ToString()
	{
            string str1 = "";
            foreach (V1Data item in Collection)
            {
                str1 += item.ToString();
            }
            return str1;
        }
						
    }

    static class Methods
    {
        static public Complex F(double x, double y)
        {
            return new Complex(x + y, x - y);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
	    const int xnode = 5;
	    const int ynode = 5;
	    const int xstep = 1;
	    const int ystep = 1;
            V1DataArray arr = new V1DataArray("V1DateArray", new DateTime(1, 1, 1),
						   xnode, ynode, xstep, ystep, Methods.F);
            Console.WriteLine(arr.ToLongString("{0:f2}\t {1:f2}\t {2:f2}\t {3:f2}\t {4:f2} \n"));
	    Console.WriteLine("Count {0} AverageValue {1:f2}\n", arr.Count, arr.AverageValue);
						
            V1DataList list = arr.ArrayToList();
            Console.WriteLine(list.ToLongString("{0:f2}\t {1:f2}\t {2:f2}\t {3:f2}\t {4:f2} \n"));
	    Console.WriteLine("Count {0} AverageValue {1:f2}\n", list.Count, list.AverageValue);

            V1MainCollection collection = new V1MainCollection();
            collection.Add(list);
	    collection.Add(arr);
            Console.WriteLine(collection.ToLongString("{0:f2}\t {1:f2}\t {2:f2}\t {3:f2}\t {4:f2} \n"));

            for (int i = 0; i < collection.Count(); i++)
            {
                Console.WriteLine("Count {0:f2} AverageValue {1:f2}", 
				      collection[i].Count, collection[i].AverageValue);
            }
						

        }
    }
}
