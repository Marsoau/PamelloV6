import pytube, sys, codecs
from pytube import extract

if (len(sys.argv) != 3):
    print("Where argumets???")
    exit(1)
ytid = sys.argv[1]
download_path = str(sys.argv[2])

class FYouTube(pytube.YouTube):
	def __init__(self, url):
		super().__init__(url=url)
		self._fast_audio = None
		self._fast_streams = []

	@property
	def fast_audio(self):
		self.check_availability()
		if self._fast_audio:
			return self._fast_audio

		self._fast_streams = []

		stream_manifest = extract.apply_descrambler(self.streaming_data)
		
		for manifest in stream_manifest:
			stream = pytube.Stream(
				stream=manifest,
				monostate=self.stream_monostate,
			)
			self._fast_streams.append(stream)

		self.stream_monostate.title = self.title
		self.stream_monostate.duration = self.length

		highest = 0
		fstream = None
		for stream in self._fast_streams:
			try:
				if (stream.type != "audio"):
					continue;
				
				current = int(stream.abr.removesuffix("kbps"))
				
				if (current > highest):
					fstream = stream
					highest = current
			except:
				continue;
		
		return fstream

video = FYouTube(f"https://www.youtube.com/watch?v={ytid}")

try:
	video.fast_audio.download("", download_path)
except:
	exit(2);