import pytube, sys, codecs
from pytube import extract
import urllib;

#CantStart 1
#WrongArgumnets 2
#InvalidYoutubeId 3
#NoAudioStreamFound 4
#AgeRestriction 5
#UnknownError 6

if (len(sys.argv) != 3):
	print("#WrongArgumnets")
	exit(2) #WrongArgumnets
ytid = sys.argv[1]
download_path = str(sys.argv[2])

if (len(ytid) != 11):
	print("#InvalidYoutubeId")
	exit(3); #InvalidYoutubeId

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
	audioStream = video.fast_audio;
	if (audioStream is None):
		print("#NoAudioStreamFound")
		exit(4); #NoAudioStreamFound
except urllib.error.HTTPError as httpError:
	print("#AgeRestriction")
	exit(5); #AgeRestriction
except pytube.exceptions.VideoUnavailable as vuError:
	print("#InvalidYoutubeId")
	exit(3); #InvalidYoutubeId

try:
	audioStream.download("", download_path)
except:
	print("#UnknownError")
	exit(6); #UnknownError