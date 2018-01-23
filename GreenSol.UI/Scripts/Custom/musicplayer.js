
$(function () {
    //$.ajaxPrefilter(function (options, originalOptions, jqXHR) { options.async = true; });
    var mouseDown = false,
       playerEle = $(".player"),
       trackList = $("#album-details p:first-child").text(),
       currentTimeEle = $("#current-time"),
       progressBarEle = $("#progress span"),
       npTitle = $("#npTitle"),
       npTrackNumber = $("#trackNumber"),
       duration = $("#duration"),

       //in order to play, this audioEle element must be associated with a DOM Audio object
       //I do this in the second $(function(){})
       audioEle = $("#audio"),
       playing = false,
       index = 0,           //current track in the list
       tracks = [],
       trackCount = 0,
       mediaPath = 'https://archive.org/download/' + trackList + '/',
       extension = '',
        //ID used in getCurrentTime();
       animID = null,
       //create a DOM Audio object, it has no source (src) yet
       //alternative: song = new Audio('data/' + url);
       supportsAudio = document.createElement("audio").canPlayType;

    /**/
    var loadTrack = function (id) {
        $('.plSel').removeClass('plSel');
        $('#plList li:eq(' + id + ')').addClass('plSel');
        npTitle.text(tracks[id].name);
        npTrackNumber.text('Track ' + tracks[id].track + '-' + trackCount);

        index = id;
        //set the source for the DOM Audio object
        //audio.src = mediaPath + tracks[id].name + extension;
        //set the source for the HTML audio element
        audioEle.attr("src", mediaPath + tracks[id].name + extension);
    };

    var playTrack = function (id) {
        loadTrack(id);
        audio.play();
    };

    //Initialization
    var initialize = function () {
        var getTracks = function () {
            if (trackList !== '') {
                //Download tracks from archive.org
                oRequest = new XMLHttpRequest();
                var sURL = "http://" + "localhost:8212"  //self.location.hostname
                            + "/Content/TrackLists/" + trackList + ".js";

                oRequest.open("GET", sURL, false);
                oRequest.send();

                tracks = JSON.parse(oRequest.responseText);
                trackCount = tracks.length;
                mediaPath = 'https://archive.org/download/' + trackList + '/';
            }
            else {
                alert("Unable to load tracks");
            }
            
        };
        getTracks();

        var buildPlaylist = function () {
            var trackNumber, trackName, trackLength;
            if (tracks.length !== 0) {
                for (i = 0; i < tracks.length ; i++) {
                    trackNumber = tracks[i].track,
                    trackName = tracks[i].name,
                    trackLength = tracks[i].length;

                    if (trackNumber.toString().length === 1) {
                        trackNumber = '0' + trackNumber;
                    }
                    else {
                        trackNumber = '' + trackNumber;
                    }
                    $('#plList').append('<li><div class="plItem"><div class="plNum">' + trackNumber + '.</div><div class="plTitle">' + trackName + '</div><div class="plLength">' + trackLength + '</div></div></li>');
                }
            }
        };
        buildPlaylist();
    };

    var reset = function () {
        if (playerEle.hasClass('paused')) {
            playerEle.removeClass('paused').addClass('playing');
        }

        progressBarEle.css('display', 'none');
        duration.text('0:00');
        getCurrentTime();
    };

    var secsToMins = function (time) {
        var int = Math.floor(time),
            mins = Math.floor(int / 60),
            secs = int % 60,
            newTime = mins + ':' + ('0' + secs).slice(-2);
        return newTime;
    };

    //update current time and progress bar as long as a track is being played 
    var getCurrentTime = function () {
        var currentTimeFormatted = secsToMins(audio.currentTime),
            currentTimePercentage = audio.currentTime / audio.duration * 100;

        currentTimeEle.text(currentTimeFormatted);
        progressBarEle.css('width', currentTimePercentage + '%');

        /*use requestAnimationFrame(someFunc) to recursively call someFunc function, 
        this is similar to setInterval() but with requestAnimation:
            The browser can optimize it, so animations will be smoother
            Animations in inactive tabs will stop, allowing the CPU to chill
            More battery-friendly
        requestAnimationFrame(someFunc) returns an ID, call cancelAnimationFrame(ID) to stop the function
        */
        if (playerEle.hasClass('playing')) {          
            animID = requestAnimationFrame(getCurrentTime);
        } else {
            cancelAnimationFrame(animID);
        }
    };

    //button play, pause, next, previous
    $('button')
        .on('click', function () {
            var self = $(this);

            if (self.hasClass('play-pause')) {
                if (playerEle.hasClass('paused')) {
                    playerEle.removeClass('paused').addClass('playing');
                    audio.play();
                    getCurrentTime();
                }
                else {
                    playerEle.removeClass('playing').addClass('paused');
                    audio.pause();
                }
            }
            if (self.hasClass('ff')) {
                if ((index + 1) < trackCount) {
                    index++;
                }
                else {
                    index = 0;
                }
                reset();
                playTrack(index);
            }
            if (self.hasClass('rw')) {
                if ((index - 1) > -1) {
                    index--;
                }
                else {
                    index = 0;
                }
                reset();
                playTrack(index);
            }
            //shuffle, repeat mode
        })

    //playback rate
    .on('mousedown', function () { })
    .on('mouseup', function () { });

    playerEle.on('mousedown mouseup', function () {
        mouseDown = !mouseDown;
    });

    progressBarEle.parent().on('click mousemove', function (e) {
        var self = $(this),
            totalWidth = self.width(),
            offsetX = e.offsetX,
            offsetPercentage = offsetX / totalWidth;

        if (mouseDown || e.type === 'click') {
            audio.currentTime = audio.duration * offsetPercentage;
            if (playerEle.hasClass('paused')) {
                progressBarEle.css('width', offsetPercentage * 100 + '%');
            }
        }
    });

    if (supportsAudio) {
        //bind the Audio object created with the HTML element audioEle 
        //so that the Audio object can use audioEle'src to specify the track
        audio = audioEle
        .on('loadedmetadata', function () {
                progressBarEle.css('display', 'block');
                var durationFormatted = secsToMins(audio.duration);
                duration.text(durationFormatted);
            })
            .on('play', function () {
                playing = true;
            })
            .on('pause', function () {
                playing = false;
            })
            .on('ended', function () {
                if ((index + 1) < trackCount) {
                    index++;
                    playTrack(index);
                }
                else {
                    reset();
                    audio.pause();
                    index = 0;
                    loadTrack(index);
                }
            })
            .get(0); //call .get(0) on audio is similar to audio[0] 

        extension = audio.canPlayType('audio/ogg') ? '.ogg' : (audio.canPlayType('audio/mpeg') ? '.mp3' : '');

        //Initilize
        initialize();
        loadTrack(0);

        $('#plList li').click(function () {
            var id = parseInt($(this).index());
            if (id !== index) {
                reset();
                playTrack(id);
            }
        });
    }
})