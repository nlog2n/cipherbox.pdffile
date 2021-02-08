using System;
using System.Collections.Generic;

namespace iTextSharp.text.pdf.fonts.cmaps 
{
    public class CMapCache {
        private static readonly Dictionary<String,CMapUniCid> cacheUniCid = new Dictionary<String,CMapUniCid>();
        private static readonly Dictionary<String,CMapCidUni> cacheCidUni = new Dictionary<String,CMapCidUni>();
        private static readonly Dictionary<String,CMapCidByte> cacheCidByte = new Dictionary<String,CMapCidByte>();
        private static readonly Dictionary<String,CMapByteCid> cacheByteCid = new Dictionary<String,CMapByteCid>();
        
        public static CMapUniCid GetCachedCMapUniCid(String name) {
            CMapUniCid cmap = null;
            lock (cacheUniCid) {
                cacheUniCid.TryGetValue(name, out cmap);
            }
            if (cmap == null) {
                cmap = new CMapUniCid();
                CMapParserEx.ParseCid(name, cmap, new CidResource());
                lock (cacheUniCid) {
                    cacheUniCid[name] = cmap;
                }
            }
            return cmap;
        }
        
        public static CMapCidUni GetCachedCMapCidUni(String name) {
            CMapCidUni cmap = null;
            lock (cacheCidUni) {
                cacheCidUni.TryGetValue(name, out cmap);
            }
            if (cmap == null) {
                cmap = new CMapCidUni();
                CMapParserEx.ParseCid(name, cmap, new CidResource());
                lock (cacheCidUni) {
                    cacheCidUni[name] = cmap;
                }
            }
            return cmap;
        }
        
        public static CMapCidByte GetCachedCMapCidByte(String name) {
            CMapCidByte cmap = null;
            lock (cacheCidByte) {
                cacheCidByte.TryGetValue(name, out cmap);
            }
            if (cmap == null) {
                cmap = new CMapCidByte();
                CMapParserEx.ParseCid(name, cmap, new CidResource());
                lock (cacheCidByte) {
                    cacheCidByte[name] = cmap;
                }
            }
            return cmap;
        }
        
        public static CMapByteCid GetCachedCMapByteCid(String name) {
            CMapByteCid cmap = null;
            lock (cacheByteCid) {
                cacheByteCid.TryGetValue(name, out cmap);
            }
            if (cmap == null) {
                cmap = new CMapByteCid();
                CMapParserEx.ParseCid(name, cmap, new CidResource());
                lock (cacheByteCid) {
                    cacheByteCid[name] = cmap;
                }
            }
            return cmap;
        }
    }
}