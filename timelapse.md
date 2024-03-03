# Timelapsing

```powershell
ffmpeg -f concat -i videos.txt -vf 'setpts=0.1*PTS' -r 60 -s 3840x2160 -c:v h264 -crf 17 -pix_fmt yuv420p -an output10x.mp4
```

## Notes
```
-f = force format
concat = concatenation action
-i = input (list of files in text file)
-vf = set video filters (this is setting it 10x speed after concatenating)
-r = frames
-s = size / resolution
-c:v = codec
-crf = constant rate factor (0-51 but 17-28 is a good range)
-pix_fmt = pixel format
-an = remove audio
```