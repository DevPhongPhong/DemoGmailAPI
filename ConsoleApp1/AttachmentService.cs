using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public class AttachmentService
    {
        public static XmlDocument GetXmlHoaDon(IEnumerable<MimeEntity> attachments)
        {
            foreach (var attachment in attachments)
            {
                try
                {
                    var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                    var fileNameExtension = fileName.Substring(fileName.Length - 4).ToLower();
                    if (fileName.Length >= 3)
                    {
                        if (fileNameExtension == ".xml")
                        {
                            string xmlString = Encoding.UTF8.GetString(Convert.FromBase64String(getBase64String(attachment.ToString())));
                            XmlDocument res = new XmlDocument();
                            res.LoadXml(xmlString);
                            return res;
                        }
                        else if (fileNameExtension == ".zip")
                        {
                            using (Stream zipStream = new MemoryStream(Convert.FromBase64String(getBase64String(attachment.ToString()))))
                            {
                                using (ZipArchive zip = new ZipArchive(zipStream))
                                {
                                    foreach (ZipArchiveEntry entry in zip.Entries)
                                        if (entry.Name.ToLower().Contains(".xml"))
                                        {
                                            var xmlStream = entry.Open();
                                            XmlDocument res = new XmlDocument();
                                            res.Load(xmlStream);

                                            if (XmlHoaDon.CheckValidXmlHoaDon(res))
                                                return res;
                                        }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            throw new Exception("Khong co hoa don trong mail");
        }

        public static void SaveXmlFile(IEnumerable<MimeEntity> attachments)
        {
            bool hasXml = false;
            foreach (var attachment in attachments)
            {
                try
                {
                    var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                    var fileNameExtension = fileName.Substring(fileName.Length - 4).ToLower();
                    if (fileName.Length >= 3)
                    {
                        if (fileNameExtension == ".xml")
                        {
                            hasXml = true;
                            using (var stream = File.Create(fileName))
                            {
                                attachment.WriteTo(stream);
                            }
                        }
                        else if (fileNameExtension == ".zip")
                        {
                            byte[] zipBytes = Convert.FromBase64String(getBase64String(attachment.ToString()));

                            using (Stream zipStream = new MemoryStream(zipBytes))
                            {
                                ZipArchive zip = new ZipArchive(zipStream);
                                foreach (ZipArchiveEntry entry in zip.Entries)
                                    if (entry.Name.ToLower().Contains(".xml"))
                                    {
                                        hasXml = true;
                                        using (var stream = File.Create(entry.Name.ToLower()))
                                        {
                                            entry.Open().CopyTo(stream);
                                            stream.Close();
                                        }
                                    }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            if (hasXml) return; else throw new Exception("Khong co hoa don trong mail");
        }

        private static string getBase64String(string s)
        {
            int start = s.IndexOf("\r\n\r\n", 0);
            return s.Substring(start + 4);
        }
    }
}
