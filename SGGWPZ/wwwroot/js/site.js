// Write your JavaScript code.
$.fn.popover.Constructor.DEFAULTS.trigger = 'click';

$(function () {
    $('[data-toggle="popover"]').popover({ html: true })
})

$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})