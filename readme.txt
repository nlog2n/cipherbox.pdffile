iTextSharp可以处理AES and RC4 encryption.也可以处理linearized PDF
从版本5.4.2开始移植

bug 1: remove password的时候运行时结果不正确/未移除密码，单步正确。 可能是PdfReader(byte[], password) 中创建source时thread safe问题。

bug 2: 另iTextSharp似乎userpassword和ownerpassword的设置搞反了。这也包括用到的其他软件例如iSafePDF. ;-)


issue: 应该支持fdf文件格式


bug-1 fix: 
因为 PdfStamper.close -> PdfStamperImp.close中如果 crypto != null则还是加密保存。
这里PdfStamperImp继承PdfWriter.
所以强制将PdfStamper.PdfStampImp as PdfWriter的属性crypto设置为空。

                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        // stamp this pdf without encryption
                        stamper.Writer.crypto = null; // bug fix: I also change PdfWriter.crypto to public
                    }

目前尚不清楚为什么单步运行和F5时crypto值不一样。


bug 3: 当有exceptions时，下一次文件读写冲突！


=====================================================================

d20130726: 将BouncyCastle cryptography library分离出来作为dll.

d20130812: 将签名类加密去除。 只支持口令加密。下一步为精简打算用自己的密码库实现。

d20130902: 将14个字体文件和en.lng移到目录./Resources/下，并在messagelocalization.cs, basefont.cs中修改文件路径。like:
public const string RESOURCE_PATH = "CipherBox.Pdf.Resources.";注意最后的小数点。

d20130910: 精简Org.BouncyCastle.Crypto， 只包含AES, CBC, MD5/SHA1 等函数。空间命名为Cipherbox.Cryptography. 最少要求dotNET framework 2.0. 
           用微软System.Security.Cryptography实现的加密类命名为Cipherbox.Cryptography.Net，最少要求为dotNet 3.5.


d20130925: 添加水印和去除水印功能。注意pdflayer和xobject

d20131010: 新的iTextsharp 5.4.4, src-xtra.zip中包含 OCGParser.cs和OCGRemover.cs，对以OCG layer生成的水印消除十分有用。 还可参考
http://stackoverflow.com/questions/17687663/itext-edit-or-remove-the-layer-on-pdf

d20131021: 已经可以添加和删除cipherbox watermark,并可以删除Adobe添加的watermark.
   下一步对用户界面进行定制。

d20131025: 为支持中文水印，又从iTextSharp网站的extras目录下载了iTextAsian-all-2.1.zip支持包，里面包含cjk_registry.properties以及字体文件。缺省引入的路径是itextpdf\text\pdf\fonts\cmaps\cjk_registry.properties。加入文件并作为embedded resources in project.
目前用的是"STSong-Light", "UniGB-UCS2-H"

d20131108: 有些软件比如PDF creator产生的水印属于inline image in page content，考虑增加此类objects的删除。具体操作符包括 BI, EI, ID.