namespace Pascal;

class G6
{

    static IGraph? parseGraph(string g6, IGraphFactory factory)
    {
        var firstByte = g6[0] - '@';

        for (int i = 1; i < g6.Length;i++){

        }
            //     let bytes = g6.split('').map(c => c.charCodeAt(0) - 63);
            // let order = bytes.shift();
            //     if (!order) return factory.newGraph(0);

            //     let graph = factory.newGraph(order);
            // let a = 0, b = 1;
            // let graphBits = 0n;
            //     bytes.forEach((byte, i) =>
            //     {
            //         let v = 32;
            //         while (v)
            //         {
            //             if (byte & v)
            //             {
            //                 graph.setEdgeValue(a, b, true);
            //             }
            //             v = v >> 1;
            //             a += 1;
            //             if (a == b)
            //             {
            //                 a = 0;
            //                 b += 1;
            //                 if (b == order) break;
            //             }
            //         }
            //     })
            //     return graph;
            return null;
    }

    static String fromGraph(IGraph g)
    {
        //     let bytes = [g.order];
        // let mask = 32;
        // let byte = 0;
        //     for (let b = 1; b<g.order; b++)
        //     {
        //         for (let a = 0; a<b; a++)
        //         {
        //             if (g.getEdgeValue(a, b))
        //             {
        //                 byte |= mask;
        //             }
        //             mask >>= 1;
        // if (mask === 0)
        // {
        //     bytes.push(byte);
        //     byte = 0;
        //     mask = 32;
        // }
        //         }
        //     }
        //     if (mask !== 32) bytes.push(byte);
        // return String.fromCharCode(...bytes.map(b => b + 63));
        return "";
    }
}