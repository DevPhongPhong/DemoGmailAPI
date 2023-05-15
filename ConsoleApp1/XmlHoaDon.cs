using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public class XmlHoaDon
    {
        public XmlDocument XmlDoc { get; set; }

        // Create From FilePath
        public XmlHoaDon(string filePath)
        {
            try
            {
                XmlDoc = new XmlDocument();
                XmlDoc.Load(filePath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Create From XmlDocument Object
        public XmlHoaDon(XmlDocument xml)
        {
            XmlDoc = xml;
        }

        public XDocument GetXDoc()
        {
            return XDocument.Parse(XmlDoc.OuterXml);
        }

        public static XDocument Schema
        {
            get
            {
                return XDocument.Parse("<HDon><DLHDon Id=\"data\"><TTChung><PBan></PBan><THDon></THDon><KHMSHDon></KHMSHDon><KHHDon></KHHDon> <SHDon></SHDon> <NLap></NLap> <DVTTe></DVTTe> <TGia></TGia> <HTTToan></HTTToan> <MSTTCGP></MSTTCGP> </TTChung> <NDHDon> <NBan> <Ten></Ten> <MST></MST> <DChi></DChi> <STKNHang></STKNHang> <TNHang></TNHang> <DCTDTu></DCTDTu> </NBan> <NMua> <HVTNMHang></HVTNMHang> </NMua> <DSHHDVu> <HHDVu> <TChat></TChat> <STT></STT> <MHHDVu></MHHDVu> <THHDVu></THHDVu> <DVTinh></DVTinh> <SLuong></SLuong> <DGia></DGia> <TLCKhau></TLCKhau> <STCKhau></STCKhau> <ThTien></ThTien> <TSuat></TSuat> <TTKhac> <TTin> <TTruong></TTruong> <KDLieu></KDLieu> <DLieu></DLieu> </TTin> <TTin> <TTruong></TTruong> <KDLieu></KDLieu> <DLieu></DLieu> </TTin> </TTKhac> </HHDVu> </DSHHDVu> <TToan> <THTTLTSuat> <THTTLTSuat> <LTSuat> <TSuat></TSuat> <ThTien></ThTien> <TThue></TThue> </LTSuat> </THTTLTSuat> </THTTLTSuat> <TgTCThue></TgTCThue> <TgTThue></TgTThue> <TTCKTMai></TTCKTMai> <TgTTTBSo></TgTTTBSo> <TgTTTBChu></TgTTTBChu> </TToan> </NDHDon> <TTKhac> <TTin> <TTruong></TTruong> <KDLieu></KDLieu> <DLieu></DLieu> </TTin> <TTin> <TTruong></TTruong> <KDLieu></KDLieu> <DLieu></DLieu> </TTin> </TTKhac> </DLHDon> <DLQRCode> </DLQRCode> <DSCKS> <NBan> <Signature Id=\"seller\" xmlns=\"http://www.w3.org/2000/09/xmldsig#\"> <SignedInfo> <CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /> <SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" /> <Reference URI=\"#data\"> <Transforms> <Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\" /> <Transform Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /> </Transforms> <DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" /> <DigestValue></DigestValue> </Reference> <Reference URI=\"#sellerTime\"> <DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" /> <DigestValue></DigestValue> </Reference> </SignedInfo> <SignatureValue> </SignatureValue> <KeyInfo> <X509Data> <X509SubjectName></X509SubjectName> <X509Certificate> </X509Certificate> </X509Data> </KeyInfo> <Object Id=\"sellerTime\"> <SignatureProperties xmlns=\"\"> <SignatureProperty Target=\"#seller\"> <SigningTime></SigningTime> </SignatureProperty> </SignatureProperties> </Object> </Signature> </NBan> <NMua /> <CCKSKhac /> </DSCKS> </HDon>");
            }
        }

        public static bool CheckValidXmlHoaDon(XmlDocument xmlDoc)
        {
            XDocument xDoc = XDocument.Parse(xmlDoc.OuterXml);

            if (!xDoc.Descendants().Attributes().All(a => Schema.Descendants().Attributes().Any(s => s.Name == a.Name)))
            {
                return false;
            }

            if (!xDoc.Descendants().Elements().All(e => Schema.Descendants().Elements().Any(s => s.Name == e.Name)))
            {
                return false;
            }
            return true;
        }

        public string GetJsonTTChung()
        {
            XmlNode node = XmlDoc.SelectSingleNode("HDon/DLHDon/TTChung"); if (node == null) throw new Exception("Không có dữ liệu");
            return JsonConvert.SerializeXmlNode(node);
        }
        public string GetJsonNBan()
        {
            XmlNode node = XmlDoc.SelectSingleNode("HDon/DLHDon/NDHDon/NBan"); if (node == null) throw new Exception("Không có dữ liệu");
            return JsonConvert.SerializeXmlNode(node);
        }
        public string GetJsonNMua()
        {
            XmlNode node = XmlDoc.SelectSingleNode("HDon/DLHDon/NDHDon/NMua"); if (node == null) throw new Exception("Không có dữ liệu");
            return JsonConvert.SerializeXmlNode(node);
        }
        public string[] GetJsonHDDVs()
        {
            XmlNodeList nodeList = XmlDoc.SelectNodes("HDon/DLHDon/NDHDon/DSHHDVu/HHDVu");
            if (nodeList == null) throw new Exception("Không có dữ liệu");
            if (nodeList.Count == 0) throw new Exception("Không có dữ liệu");
            if (nodeList[0] == null) throw new Exception("Không có dữ liệu");
            string[] res = new string[nodeList.Count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = JsonConvert.SerializeXmlNode(nodeList[i]);
            }
            return res;
        }
        public string[] GetJsonTTKhacs()
        {
            XmlNodeList nodeList = XmlDoc.SelectNodes("HDon/DLHDon/TTKhac/TTin");
            if (nodeList == null) throw new Exception("Không có dữ liệu");
            if (nodeList.Count == 0) throw new Exception("Không có dữ liệu");
            if (nodeList[0] == null) throw new Exception("Không có dữ liệu");
            string[] res = new string[nodeList.Count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = JsonConvert.SerializeXmlNode(nodeList[i]);
            }
            return res;
        }

        public string GetJsonSignatureNBan()
        {
            XmlNode node = XmlDoc.SelectSingleNode("HDon/DSCKS/NBan").FirstChild;
            if (node == null) throw new Exception("Không có dữ liệu");
            return JsonConvert.SerializeXmlNode(node);
        }

        public string GetJsonSignatureNMua()
        {
            XmlNode node = XmlDoc.SelectSingleNode("HDon/DSCKS/NMua").FirstChild;
            if (node == null) throw new Exception("Không có dữ liệu");
            return JsonConvert.SerializeXmlNode(node);
        }

        public string GetJsonCCKSKhac()
        {
            XmlNode node = XmlDoc.SelectSingleNode("HDon/DSCKS/CCKSKhac");
            return JsonConvert.SerializeXmlNode(node);
        }

        public DateTime GetJsonNBanSignedTime()
        {
            JObject jobject = JObject.Parse(GetJsonSignatureNBan());
            return jobject.SelectToken("Signature.Object.SignatureProperties.SignatureProperty.SigningTime").Value<DateTime>();
        }

        public DateTime GetJsonNMuaSignedTime()
        {
            JObject jobject = JObject.Parse(GetJsonSignatureNMua());
            return jobject.SelectToken("Signature.Object.SignatureProperties.SignatureProperty.SigningTime").Value<DateTime>();
        }
    }
}