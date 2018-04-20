import 'es6-collections';
import 'element-theme-default/lib/index.css';

import Vue from "vue";
import MessageList from './components/messagelist';
import SessionList from './components/sessionlist';
import MessageView from './components/messageview';
import SessionView from './components/sessionview';
import Component from "vue-class-component";
import MessageSummary from "ApiClient/MessageSummary";
import SessionSummary from "ApiClient/SessionSummary";

import Quasar from 'quasar-framework/dist/quasar.mat.esm.js';
import axios from 'axios' 

Vue.use(Quasar);

@Component({})
export default class Main extends Vue {

    selectedMessage: MessageSummary | null = null;
    selectedSession: SessionSummary | null = null;

    selectedMessageChanged(selectedMessage: MessageSummary | null) {
        this.selectedMessage = selectedMessage;
    };

    selectedSessionChanged(selectedSession: SessionSummary | null) {
        this.selectedSession = selectedSession;
    };

}

Quasar.start(() => {
    new Main({
        el: '#app',
        components: { messagelist: MessageList, messageview: MessageView, sessionlist: SessionList, sessionview: SessionView }
    });
});