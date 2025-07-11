/* ========================= Header Fixed when Scroll ========================= */
window.onscroll = function() {fixHeader(); scrollFunction()};
var header = document.getElementById("header");
var sticky = header.offsetTop;
function fixHeader() {
	if (window.pageYOffset > sticky) {
		header.classList.add("sticky");
	} else {
		header.classList.remove("sticky");
	}
}

/* ========================= Back to Top ========================= */
let mybutton = document.getElementById("clickToTop");
function scrollFunction() {
  if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
    mybutton.style.display = "block";
  } else {
    mybutton.style.display = "none";
  }
}

function topFunction() {
  document.body.scrollTop = 0; // For Safari
  document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
} 

/* ========================= Mobile Canvas ========================= */
function darken_screen(yesno){
	if( yesno == true ){
		document.querySelector('.screen-darken').classList.add('active');
	}
	else if(yesno == false){
		document.querySelector('.screen-darken').classList.remove('active');
	}
}

function close_offcanvas(){
	darken_screen(false);
	document.querySelector('.mobile-offcanvas.show').classList.remove('show');
	document.body.classList.remove('offcanvas-active');
}

function show_offcanvas(offcanvas_id){
	darken_screen(true);
	document.getElementById(offcanvas_id).classList.add('show');
	document.body.classList.add('offcanvas-active');
}

document.addEventListener("DOMContentLoaded", function(){
	document.querySelectorAll('[data-trigger]').forEach(function(everyelement){
		let offcanvas_id = everyelement.getAttribute('data-trigger');
		everyelement.addEventListener('click', function (e) {
			e.preventDefault();
			show_offcanvas(offcanvas_id);
		});
	});

	document.querySelectorAll('.btn-close').forEach(function(everybutton){
		everybutton.addEventListener('click', function (e) {
			e.preventDefault();
			close_offcanvas();
		});
	});

	document.querySelector('.screen-darken').addEventListener('click', function(event){
		close_offcanvas();
	});	
}); 

document.addEventListener('DOMContentLoaded', function() {
    // Lấy tất cả các phần tử <li> có link bên trong
    const listItems = document.querySelectorAll('.list-post li a');

    // Lặp qua tất cả các link và thêm sự kiện click
    listItems.forEach(function(link) {
        link.addEventListener('click', function() {
            // Xóa class "active" khỏi tất cả các <li>
            listItems.forEach(function(l) {
                l.parentElement.classList.remove('active');
            });

            // Thêm class "active" vào <li> của link được click
            this.parentElement.classList.add('active');
        });
    });
});