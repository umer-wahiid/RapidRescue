(function ($) {
    "use strict";
    var $window = $(window);
    $('#preloader').fadeOut('normall', function () {
        $(this).remove();
    });
    $window.on('scroll', function () {
        var scroll = $window.scrollTop();
        var innerlogo = $(".navbar-brand.inner-logo img");
        if (scroll <= 50) {
            $("header").removeClass("scrollHeader").addClass("fixedHeader");
            innerlogo.attr('src', '/user/img/logos/amblogo.png');
        } else {
            $("header").removeClass("fixedHeader").addClass("scrollHeader");
            if ($window.width() >= 992) {
                innerlogo.attr('src', '/user/img/logos/darklogo.png');
            }
        }
    });
    $window.on('scroll', function () {
        if ($(this).scrollTop() > 500) {
            $(".scroll-to-top").fadeIn(400);
        } else {
            $(".scroll-to-top").fadeOut(400);
        }
    });
    $(".scroll-to-top").on('click', function (event) {
        event.preventDefault();
        $("html, body").animate({
            scrollTop: 0
        }, 600);
    });
    var pageSection = $(".parallax,.bg-img");
    pageSection.each(function (indx) {
        if ($(this).attr("data-background")) {
            $(this).css("background-image", "url(" + $(this).data("background") + ")");
        }
    });
    $('.story-video').magnificPopup({
        delegate: '.video',
        type: 'iframe'
    });
    $window.resize(function (event) {
        setTimeout(function () {
            SetResizeContent();
        }, 500);
        event.preventDefault();
    });

    function fullScreenHeight() {
        var element = $(".full-screen");
        var $height = $window.height();
        element.css('height', $height);
    }

    function ScreenFixedHeight() {
        var $headerHeight = $("header").height();
        var element = $(".screen-height");
        var $screenheight = $window.height() - $headerHeight;
        element.css('height', $screenheight);
    }

    function SetResizeContent() {
        fullScreenHeight();
        ScreenFixedHeight();
    }
    SetResizeContent();
    $(document).on("ready", function () {
        $('.client').owlCarousel({
            loop: true,
            responsiveClass: true,
            autoplay: true,
            smartSpeed: 800,
            nav: false,
            dots: true,
            center: false,
            margin: 0,
            responsive: {
                0: {
                    items: 1,
                    dots: false
                },
                768: {
                    items: 1
                },
                992: {
                    items: 2
                }
            }
        });
        $('#clients').owlCarousel({
            loop: true,
            nav: false,
            dots: false,
            autoplay: true,
            autoplayTimeout: 3000,
            responsiveClass: true,
            autoplayHoverPause: false,
            responsive: {
                0: {
                    items: 2,
                    margin: 20
                },
                768: {
                    items: 3,
                    margin: 40,
                },
                992: {
                    items: 4,
                    margin: 60,
                },
                1200: {
                    items: 5,
                    margin: 80,
                }
            }
        });
        $('.owl-carousel').owlCarousel({
            items: 1,
            loop: true,
            dots: false,
            margin: 0,
            autoplay: true,
            smartSpeed: 500
        });
        if ($("#rev_slider_2").length !== 0) {
            var tpj = jQuery;
            var revapi2;
            tpj(document).ready(function () {
                if (tpj("#rev_slider_2").revolution == undefined) {
                    revslider_showDoubleJqueryError("#rev_slider_2");
                } else {
                    revapi2 = tpj("#rev_slider_2").show().revolution({
                        sliderType: "standard",
                        sliderLayout: "fullwidth",
                        dottedOverlay: "none",
                        delay: 9000,
                        spinner: "spinner4",
                        navigation: {
                            keyboardNavigation: "off",
                            keyboard_direction: "horizontal",
                            mouseScrollNavigation: "off",
                            onHoverStop: "off",
                            touch: {
                                touchenabled: "on",
                                swipe_threshold: 75,
                                swipe_min_touches: 1,
                                swipe_direction: "horizontal",
                                drag_block_vertical: false
                            },
                            arrows: {
                                enable: true,
                                style: 'metis',
                                tmp: '',
                                rtl: false,
                                hide_onleave: true,
                                hide_onmobile: true,
                                hide_under: 0,
                                hide_over: 9999,
                                hide_delay: 200,
                                hide_delay_mobile: 1200,
                                left: {
                                    container: 'slider',
                                    h_align: 'left',
                                    v_align: 'center',
                                    h_offset: 20,
                                    v_offset: 0
                                },
                                right: {
                                    container: 'slider',
                                    h_align: 'right',
                                    v_align: 'center',
                                    h_offset: 20,
                                    v_offset: 0
                                }
                            },
                            bullets: {
                                enable: false,
                            },
                        },
                        responsiveLevels: [1240, 1024, 767, 480],
                        gridwidth: [1170, 1170, 767, 480],
                        gridheight: [700, 700, 600, 600],
                        lazyType: "none",
                        shadow: 0,
                        shuffle: "off",
                        autoHeight: "off",
                    });
                }
            });
        }
        $('.countup').counterUp({
            delay: 25,
            time: 2000
        });
        $(".countdown").countdown({
            date: "01 Jan 2024 00:01:00",
            format: "on"
        });
    });
    $window.on("load", function () {
        $('.gallery').magnificPopup({
            delegate: '.popimg',
            type: 'image',
            gallery: {
                enabled: true
            }
        });
        var $gallery = $('.gallery').isotope({});
        $('.filtering').on('click', 'span', function () {
            var filterValue = $(this).attr('data-filter');
            $gallery.isotope({
                filter: filterValue
            });
        });
        $('.filtering').on('click', 'span', function () {
            $(this).addClass('active').siblings().removeClass('active');
        });
        $window.stellar();
    });
})(jQuery);