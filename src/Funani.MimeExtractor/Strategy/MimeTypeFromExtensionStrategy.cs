﻿
namespace Funani.MimeExtractor.Strategy
{
    using System;
    using System.IO.Abstractions;

    internal class MimeTypeFromExtensionStrategy : IMimeTypeExtractor
    {
        public string ExtractMimeType(IFileInfo file)
        {
        	if (file.Name.ToLowerInvariant() == "thumbs.db")
        		return "application/x-msthumbnails";
        	
            String mime;
            String extension = file.Extension.ToLowerInvariant();
            switch (extension)
            {
                case ".avd":
                    mime = "application/x-magixmoviedata";
                    break;
                case ".hdp":
                    mime = "application/x-magixaudiodata";
                    break;
                case ".mxc2":
                    mime = "application/x-magixmediapreview";
                    break;
                    
                case ".avi":
                    mime = "video/x-msvideo";
                    break;
                case ".bmp":
                    mime = "image/bmp";
                    break;
                case ".cab":
                    mime = "application/vnd.ms-cab-compressed";
                    break;
                case ".css":
                    mime = " text/css";
                    break;
                case ".doc":
                    mime = "application/msword";
                    break;
                case ".exe":
                    mime = "application/exe";
                    break;
                case ".gif":
                    mime = "image/gif";
                    break;
                case ".hps-metadata":
                    mime = "application/xml";
                    break;
                case ".ini":
                case ".txt":
                    mime = "text/plain";
                    break;
                case ".htm":
                case ".html":
                    mime = "text/html";
                    break;
                case ".xml":
                    mime = "text/xml";
                    break;
                case ".iso":
                    mime = "application/iso-image";
                    break;
                case ".jpe":
                case ".jpeg":
                case ".jpg":
                    mime = "image/jpeg";
                    break;
                case ".lnk":
                    mime = "application/x-ms-shortcut";
                    break;
                case ".mov":
                case ".qt":
                    mime = "video/quicktime";
                    break;
                case ".mpe":
                case ".mpeg":
                case ".mpg":
                    mime = "video/mpeg";
                    break;
                case ".pdf":
                    mime = "application/pdf";
                    break;
                case ".php":
                    mime = "text/x-php";
                    break;
                case ".odg":
                    mime = "application/vnd.oasis.opendocument.graphics";
                    break;
                case ".ods":
                    mime = "application/vnd.oasis.opendocument.spreadsheet";
                    break;
                case ".odt":
                    mime = "application/vnd.oasis.opendocument.text";
                    break;
                case ".png":
                    mime = "image/png";
                    break;
                case ".pps":
                case ".ppt":
                    mime = "application/vnd.ms-powerpoint";
                    break;
                case ".rtf":
                    mime = "application/rtf";
                    break;
                case ".tif":
                case ".tiff":
                    mime = "image/tiff";
                    break;
                case ".wps":
                    mime = "application/vnd.ms-works";
                    break;
                case ".xcf":
                    mime = "application/gimp";
                    break;
                case ".xls":
                    mime = "application/msexcel";
                    break;
                case ".gz":
                    mime = "application/gzip";
                    break;
                case ".zip":
                    mime = "application/zip";
                    break;
                case "":
                case ".db":
                case ".indexarrays":
                case ".indexgroups":
                case ".shadowindexgroups":
                case ".indexhead":
                case ".shadowindexhead":
                case ".indexids":
                case ".indexdirectory":
                case ".shadowindexdirectory":
                case ".indexpostings":
                case ".indexpositions":
                case ".indexpositiontable":
                case ".indexcompactdirectory":
                case ".plist": // MAC OS X trash folder
                case ".trashes": // MAC OS X trash folder
                case ".tag":
                case ".hdr":
                case ".dat":
                case ".bin":
                case ".dll":
                    mime = "application/octet-stream";
                    break;
                default:
                    mime = "application/octet-stream";
                    System.Diagnostics.Trace.TraceWarning("MIME type not recognized for file '{0}'", file.Name);
                    break;
            }
            return mime;
        }
    }
}
