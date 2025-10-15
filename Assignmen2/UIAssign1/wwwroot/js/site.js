// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function() {
  const row = document.getElementById('homeProducts');
  if (!row) return;

  const prevBtn = document.getElementById('productPrev');
  const nextBtn = document.getElementById('productNext');

  function getCardWidth() {
    const first = row.querySelector('[class*="col-"]');
    if (!first) return 300;
    const style = window.getComputedStyle(first);
    return first.getBoundingClientRect().width + parseFloat(style.marginRight || '0');
  }

  function scrollByOne(forward) {
    const step = getCardWidth();
    row.scrollBy({ left: forward ? step : -step, behavior: 'smooth' });
  }

  prevBtn && prevBtn.addEventListener('click', function() { scrollByOne(false); });
  nextBtn && nextBtn.addEventListener('click', function() { scrollByOne(true); });
})();