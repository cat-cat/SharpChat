'use strict';

import User from '../models/user.model';
import upload from '../config/multer/user';
import cfg from '../config/env';
import imghlp from '../helpers/imgcompress';

function get (req, res) {
    User.get({_id: req.user._id}, {__v: 0, token: 0})
        .then(user => res.json({userAvatarPrefix:cfg.userAvatarPrefix(), user}))
        .catch(e => res.json({success: false}))
}

function getAll (req, res) {
    User.getAll({}, {})
        .then(users => res.json({userAvatarPrefix:cfg.userAvatarPrefix(), users}))
        .catch(e => res.json({success: false}))
}

/**
 * Create user and make JWT token
 * @param req {req} - Request
 * @param res {res} - Response
 */
function create (req, res) {
    upload(req, res, (e) => {
    	var user = req.body;
    	user = (req.file) ? Object.assign(user, {image: req.file.filename }) : user;
        (e) ? res.json({ success: false, message: e }) :

		    User.create(user)
		        .then(user => res.json({success: true, token: user.token}))
		        .catch(e => res.json({success: false, error: e}))

        if(req.file)
        	imghlp.imgcompress(req.file, '/uploads/users')
    });
}

function addPushId(req, res) {
	User.addPushId(req.user._id, req.query.push_id)
        .then(user => res.json({success: true}))
        .catch(e => res.json({success: false, error: e}))
}

/**
 * Edit data user
 * @param req {req} - Request
 * @param res {res} - Response
 */
function edit (req, res) {
    upload(req, res, (e) => {

    	var user = req.body;
    	user = (req.file) ? Object.assign(user, {image: req.file.filename }) : user;
        (e) ? res.json({ success: false, message: e }) :

	    User.edit({ _id: req.user._id }, user)
	        .then(user => res.json({success: true}))
	        .catch(e => res.json({success: false, error: e}))

        if(req.file)
        	imghlp.imgcompress(req.file, '/uploads/users')
    });
}

export default { create, get, edit, getAll, addPushId };
