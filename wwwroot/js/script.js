

(function ($) {

    (function(){
        var lang = (document.documentElement.getAttribute('lang') || 'en').split('-')[0].toLowerCase();

        String.prototype.language = function(){
            var arr = this.split(/\[:(\w.)?\]/g);
            arr.shift();
            var langs = {};
            for(var i = 0, j = arr.length; i < j; i += 2){
                if(arr[i]) langs[arr[i]] = arr[i + 1];
            }   
            return langs;
        }

        String.languageFrom = function(obj){
            var str = '';
            $.each(obj,function(k,v){ str += '[:'+ k +']' + v });
            return str;
        }

        String.prototype.i18n = function(language){
            language = language || lang;
            return this.language()[language] || this.toString();
        }
    })();

    $(document).ready(function () {
    	// Thumbnail
		if ($(".middle_img").length > 0) {
			$('.middle_img').each(
			    function(){
			        var height = $(this).find('img').outerHeight(),
			        	width = $(this).find('img').outerWidth();
			        if(height > width){
			        	$(this).find('img').css({'width': '100%'});
			        }else{
			        	$(this).find('img').css({'height': '100%'});
			        }
			    }
			)
		}

        /* Menu mobile */
        $('.menu-btn').click(function(){
            if($('body').hasClass('showMenu')){
                $('body').removeClass('showMenu');
            }else{
                $('body').addClass('showMenu');
                //$('.flexMenuToggle:first').click();
            return false;
            }
        }); 
    

    // Click scroll a
    $(".scroll").on('click', function (event) {
      event.preventDefault();
      $('html,body').animate({scrollTop: 0 }, 1000);
    });

    // Search
    $('.btnSearch').click(function(){
        $('body').toggleClass('showSearch');
    });    


    if ($(".select-chosen").length > 0) {
        $('.select-chosen').chosen();
    }

        // Add height
        if ($(".TOA007 .item").length > 0) {
            $('.TOA007 .item').each(
                function(){
                    var width = $(this).outerWidth();
                    $(this).css({'height': width/1.5});
                }
            )
        }

        if ($(".TOA041 .block_item").length > 0) {
            $('.TOA041 .block_item').each(
                function(){
                    var width = $(this).outerWidth();
                    $(this).css({'height': width/1.5});
                }
            )
        }


        //responsive
        if ($(window).width() < 768) {

            // Add height
            if ($("#fullVideo .item_video").length > 0) {
                        var height = $("#fullVideo   .item_video").outerHeight();
                        $("#fullVideo iframe").css({'height':height});

            }


            if ($(".TOA015 .block_item").length > 0) {
                $('.TOA015 .block_item').each(
                    function(){
                        var width = $(this).outerWidth();
                        $(this).css({'height': width/1.7});
                    }
                )
            }

        }


        //OWL----------------------------------------------------
        $('.single-slide').each(function () {
            $(this).owlCarousel({
                navText: ["<i class='fa fa-angle-left'></i>","<i class='fa fa-angle-right'></i>"],
                items: 1,
                autoplayHoverPause:false,
                autoplay: $(this).hasClass('s-auto') ? true : false,
                dots: $(this).hasClass('s-dots') ? true : false,
                loop: true,
                lazyLoad: $(this).hasClass('s-lazy') ? true : false,
                nav: $(this).hasClass('s-nav') ? true : false,
                stopOnHover: false
          
                // autoplaySpeed:1000
            })
        });

        $('.slide-center, .slide-center2').owlCarousel({
            navText: ["<i class='fa fa-angle-left'></i>","<i class='fa fa-angle-right'></i>"],
            center: true,
            items:2,
            loop:true,
            dots:false,
            margin:0,
            autoplay:true,
            nav:true,
            responsive : {
                0 : {
                        center: false,
                        items:1,
                },
                768 : {
                        margin:10,
                        padding:150,
                },
                1200 : {
                        margin:20,
                        padding:250,
                }    
            }
        });




        $('.slide4-arr').owlCarousel({
            navText: ["<i class='fa fa-angle-left'></i>","<i class='fa fa-angle-right'></i>"],
            nav:true,
            dots:false,
            responsive:{
                0:{
                    items:1
                },
                600:{
                    items:2
                },
                800:{
                    items:3
                },
                1200:{
                    items:4
                }

            }            
        });

        $('.slide5-arr').owlCarousel({
            navText: ["<i class='fa fa-angle-left'></i>","<i class='fa fa-angle-right'></i>"],
            nav:true,
            dots:false,
            responsive:{
                0:{
                    items:2
                },
                600:{
                    items:3
                },
                800:{
                    items:4
                },
                1200:{
                    items:5
                }

            }            
        });

        // END OWL----------------------------------------------------

        // iframe video
        var getVideoUrl = function(id){
            return 'https://www.youtube.com/embed/' + id + '?rel=0&autoplay=1';
        }
        var addVideoUrl = function(id){
            return 'https://www.youtube.com/embed/' + id + '?rel=0';
        }
        var closeVideoUrl = function(id){
            return 'https://www.youtube.com/embed/' + id + '?rel=0';
        }


        var modal = $('#myVideo').on('hidden.bs.modal',function(){
            modal.find('iframe').removeAttr('src');
        });
        var fullVideo = $('#fullVideo');

        
        $(document).on('click','.item_video',function(){
            var btnVideo = $(this);    
            var video = btnVideo.attr('data-video');     
            //modal.find('iframe').attr('src', getVideoUrl(video));
            fullVideo.addClass('show-video');
            fullVideo.find('iframe').attr('src', getVideoUrl(video));
        });  

        //Sự kiện tắt video, hiển thị lại video top
        $(".close-video").unbind('click').on("click",function(){
            var video = $(this).attr('data-video');     
            fullVideo.addClass('show-video');
            fullVideo.find('iframe').attr('src', closeVideoUrl(video));
        })


        // single video
        $(document).on('click','.single-video .item_video',function(){
            var btnVideo = $(this);    
            var video = btnVideo.attr('data-video');
            var parent = btnVideo.parent().parent();     
            if(parent.hasClass('show-video')){
                    parent.removeClass('show-video');
                    parent.find('iframe').removeAttr('src');
            }else{
                    parent.addClass('show-video');
                    parent.find('iframe').attr('src', getVideoUrl(video));
            };  
        });  




        // slideToggle
        $('.block-toggle .block-title').each(function(){
            var btnTg = $(this).click(function(){ 
                    if(btnTg.parent().hasClass('active')){
                        btnTg.parent().removeClass('active');
                        btnTg.parent().find('.block-content').slideUp(300);                                           
                    }else{
                        $('.block-toggle .block-item').removeClass('active');
                        $('.block-toggle .block-content').slideUp(300);
                        btnTg.parent().addClass('active');
                        btnTg.parent().find('.block-content').slideDown(300);               
                    };  
            });
        });       




		// Menu
		$('ul.mainmenu li.parent, #footer ul.menu li.parent ').each(function(){
		     var 
                p = $(this),
                btn = $('<span>',{'class':'showsubmenu fa fa-caret-down', text : '' }).click(function(){
    				if(p.hasClass('parent-showsub')){
    				    menu.slideUp(300,function(){
    				        p.removeClass('parent-showsub');
    				    });        										
    				}else{
    				    menu.slideDown(300,function(){
    				        p.addClass('parent-showsub');
    				    });        										
    				};	
    			}),
                menu = p.children('ul')
             ;
             p.prepend(btn) 
		});	

        //equalHeight
        if ($(".block-9  .item").length > 0) {
          $('.block-9  .item').imagesLoaded(function () {
            equalHeight(".block-9  .item", 60);
          });

        }
        if ($(".TOA027  .item").length > 0) {
          $('.TOA027  .item').imagesLoaded(function () {
            equalHeight(".TOA027  .item", 60);
          });

        }


        // section background
        $('.section-background').each(function(){
            var filename = $(this).find('.img-background').attr('src');
            if(typeof filename ==='undefined'){
                console.log('fack ' + filename);
            }else{
                $(this).css('background-image', 'url(' + filename + ')');
            }

        })  

    $('.top-detail .btnshow').click(function(){
        if($('.top-detail').hasClass('showmore-desc')){
            $('.top-detail').removeClass('showmore-desc');
        }else{
            $('.top-detail').addClass('showmore-desc');
        }
    });

    $('.gallery .owl-carousel').owlCarousel({
        loop:true,
        items:1,
        dots: false,
        nav:true,
        center: false,
        autoplay: true,
        stagePadding: 250,
        navText: ["<i class='fa fa-angle-left'></i>","<i class='fa fa-angle-right'></i>"],
        margin:25,
        thumbs: false,
        responsive:{
            0:{
                stagePadding: 0,
                margin: 0,
                nav:false,
            },
            600:{
                stagePadding: 150,
            },
            1000:{
                stagePadding: 250,

            },
            1600:{
                stagePadding: 350,
                
            },
            1920:{
                stagePadding: 450,
              
            },
            2560:{
                stagePadding: 550,
            }
        }
    });

     $('.overflow').each(function(){
        $(this).click(function(){
            $(this).addClass('show-menu');
        });
    });


  
        // 
        if ($(".TOA015 .block_item").length > 0) {
              var tag = document.createElement('script');
              tag.src = "https://www.youtube.com/iframe_api";
              var firstScriptTag = document.getElementsByTagName('script')[0];
              firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

            var blockItems = $('.TOA015 .block_item').each(function(){
                var clas = $(this).attr('data-class');
                var parent = $(this).parent();
                var imgactive = $(this).parent().children('div.' + clas).attr('data-active');                
                var iframe = $(this).parent().children('div.' + clas);
                var idVideo = iframe.attr('data-video');
                var fullImg = $(this).parent().children('div.' + clas).children('.activecontent');
                var item = $(this).hover(function(){ 
                        item.addClass('active');
                        parent.children('div.' + clas).addClass('active');    //children('img.' + clas)
                        fullImg.css('background-image', 'url(' + imgactive + ')');    //add img active
                        if(idVideo){
                            if(this.player){

                                this.player.playVideo && this.player.playVideo();

                            }else{

                                var player =this.player = new YT.Player(iframe.children().get(0),{
                                    height: '390',
                                    width: '640',
                                    videoId: idVideo,
                                    events: {
                                        onReady: function(){
                                            if(iframe.is(':visible')) player.playVideo();
                                        }
                                    }
                                });   
                            }
                        }
                    },
                    function() {
                        parent.children().removeClass('active'); 
                        this.player && this.player.pauseVideo && this.player.pauseVideo()
                    }    
                );
            });
        }




        /* Equal Height good*/
        function equalHeight(className, padding) {
          var tempHeight = 0;
          $(className).each(function () {
            current = $(this).height();
            if (parseInt(tempHeight) < parseInt(current)) {
              tempHeight = current;
            }
          });
          $(className).css("minHeight", tempHeight + padding + "px");
        }


        
        $(window).bind("load", function() {
            // Lazy img 
            $('.img-lazy').each(function(){
                var src = $(this).attr('data-src');
                $(this).attr('src', src);
                $(this).removeClass('img-lazy').addClass('img-loaded');
            }); // end Lazy img 
            // Lazy bg 
            $('.bg-lazy').each(function(){
                var src = $(this).attr('data-src');
                $(this).css('background-image', 'url(' + src + ')');
                $(this).removeClass('bg-lazy').addClass('bg-loaded');
            }); // end Lazy bg 

            // Facebook
            $('.fb-messenger .btnFb').click(function(){

                $('.fb-messenger').toggleClass('active');

                (function(d, s, id) {
                    var js, fjs = d.getElementsByTagName(s)[0];
                    if (d.getElementById(id)) return;
                    js = d.createElement(s); js.id = id;
                    js.src = "//connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.0";
                    fjs.parentNode.insertBefore(js, fjs);
                }(document, 'script', 'facebook-jssdk'));


            }); 


        });        

     

    });
})(jQuery);	