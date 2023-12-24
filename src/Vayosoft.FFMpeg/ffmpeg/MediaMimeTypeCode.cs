using System.Drawing.Imaging;
using System.Runtime.Serialization;

namespace Vayosoft.FFMpeg.ffmpeg
{
    public enum MediaMimeTypeCode
    {
         UNDEFINED = 0,
         PNG = 10,
         GIF = 11,
         BMP,
         MP4 = 21,
         METADATA = 41,
         MP3 = 31
    }

    public enum VayoMediaType
    {
        
        UNDEFINED = 0,
        
        IMAGE = 1,
        
        VIDEO = 2,
        
        AUDIO = 3,
        
        TEXT = 4
    }

    public static class EMediaTypeExtention
    {
        public static VayoMediaType MediaType(this MediaMimeTypeCode mimeType)
        {
            switch (((int)mimeType) / 10)
            {
                case 1:
                    return VayoMediaType.IMAGE;
                case 2:
                    return VayoMediaType.VIDEO;
                case 3:
                    return VayoMediaType.AUDIO;
                case 4:
                    return VayoMediaType.TEXT;
                default:
                    return VayoMediaType.UNDEFINED;
            }
        }

        public static string GetExtention(this MediaMimeTypeCode type)
        {
            switch (type)
            {
                case MediaMimeTypeCode.GIF:
                    return ".gif";
                case MediaMimeTypeCode.MP4:
                    return ".mp4";
                case MediaMimeTypeCode.PNG:
                    return ".png";
                case MediaMimeTypeCode.MP3:
                    return ".mp3";
                default:
                    return "";
            }
        }

        public static string ToString(this MediaMimeTypeCode type)
        {
            switch (type)
            {
                case MediaMimeTypeCode.GIF:
                    return "image/gif";
                case MediaMimeTypeCode.MP4:
                    return "video/mp4";
                case MediaMimeTypeCode.PNG:
                    return "image/png";
                case MediaMimeTypeCode.BMP:
                    return "image/bmp";
                case MediaMimeTypeCode.MP3:
                    return "audio/mp3";
                default:
                    return "application/octet-stream";
            }
        }

        public static ImageFormat GetImageFormat(this MediaMimeTypeCode type)
        {
            switch (type)
            {
                case MediaMimeTypeCode.GIF:
                    return ImageFormat.Gif;
                case MediaMimeTypeCode.PNG:
                    return ImageFormat.Png;
                case MediaMimeTypeCode.BMP:
                    return ImageFormat.Bmp;
                default:
                    return ImageFormat.Jpeg;
            }
        }
    }
}
