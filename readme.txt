iTextSharp���Դ���AES and RC4 encryption.Ҳ���Դ���linearized PDF
�Ӱ汾5.4.2��ʼ��ֲ

bug 1: remove password��ʱ������ʱ�������ȷ/δ�Ƴ����룬������ȷ�� ������PdfReader(byte[], password) �д���sourceʱthread safe���⡣

bug 2: ��iTextSharp�ƺ�userpassword��ownerpassword�����ø㷴�ˡ���Ҳ�����õ��������������iSafePDF. ;-)


issue: Ӧ��֧��fdf�ļ���ʽ


bug-1 fix: 
��Ϊ PdfStamper.close -> PdfStamperImp.close����� crypto != null���Ǽ��ܱ��档
����PdfStamperImp�̳�PdfWriter.
����ǿ�ƽ�PdfStamper.PdfStampImp as PdfWriter������crypto����Ϊ�ա�

                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        // stamp this pdf without encryption
                        stamper.Writer.crypto = null; // bug fix: I also change PdfWriter.crypto to public
                    }

Ŀǰ�в����Ϊʲô�������к�F5ʱcryptoֵ��һ����


bug 3: ����exceptionsʱ����һ���ļ���д��ͻ��


=====================================================================

d20130726: ��BouncyCastle cryptography library���������Ϊdll.

d20130812: ��ǩ�������ȥ���� ֻ֧�ֿ�����ܡ���һ��Ϊ����������Լ��������ʵ�֡�

d20130902: ��14�������ļ���en.lng�Ƶ�Ŀ¼./Resources/�£�����messagelocalization.cs, basefont.cs���޸��ļ�·����like:
public const string RESOURCE_PATH = "CipherBox.Pdf.Resources.";ע������С���㡣

d20130910: ����Org.BouncyCastle.Crypto�� ֻ����AES, CBC, MD5/SHA1 �Ⱥ������ռ�����ΪCipherbox.Cryptography. ����Ҫ��dotNET framework 2.0. 
           ��΢��System.Security.Cryptographyʵ�ֵļ���������ΪCipherbox.Cryptography.Net������Ҫ��ΪdotNet 3.5.


d20130925: ���ˮӡ��ȥ��ˮӡ���ܡ�ע��pdflayer��xobject

d20131010: �µ�iTextsharp 5.4.4, src-xtra.zip�а��� OCGParser.cs��OCGRemover.cs������OCG layer���ɵ�ˮӡ����ʮ�����á� ���ɲο�
http://stackoverflow.com/questions/17687663/itext-edit-or-remove-the-layer-on-pdf

d20131021: �Ѿ�������Ӻ�ɾ��cipherbox watermark,������ɾ��Adobe��ӵ�watermark.
   ��һ�����û�������ж��ơ�

d20131025: Ϊ֧������ˮӡ���ִ�iTextSharp��վ��extrasĿ¼������iTextAsian-all-2.1.zip֧�ְ����������cjk_registry.properties�Լ������ļ���ȱʡ�����·����itextpdf\text\pdf\fonts\cmaps\cjk_registry.properties�������ļ�����Ϊembedded resources in project.
Ŀǰ�õ���"STSong-Light", "UniGB-UCS2-H"

d20131108: ��Щ�������PDF creator������ˮӡ����inline image in page content���������Ӵ���objects��ɾ����������������� BI, EI, ID.